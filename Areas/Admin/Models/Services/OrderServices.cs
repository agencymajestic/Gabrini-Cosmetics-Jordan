using GabriniCosmetics.Areas.Admin.Models.Interface;
using GabriniCosmetics.Data;
using Microsoft.EntityFrameworkCore;

namespace GabriniCosmetics.Areas.Admin.Models.Services
{
    public class OrderServices : IOrder
    {
        private readonly GabriniCosmeticsContext _context;

        public OrderServices(GabriniCosmeticsContext context)
        {
            _context = context;
        }

        public async Task<Order> CreateOrder(Order order)
        {
            _context.Entry(order).State = EntityState.Added;
            await _context.SaveChangesAsync();

            return order;
        }
        public async Task<Order> EditOrder(int id, Order order)
        {
            // Retrieve the existing order from the database based on the ID
            var existingOrder = await _context.Orders.FindAsync(id);
            if (existingOrder == null)
            {
                // Handle the case where the order with the specified ID is not found.
                throw new Exception("Order not found.");
            }

            // Update the PaymentStatus property of the existing order with the new value
            existingOrder.PaymentStstus = order.PaymentStstus;
            existingOrder.OrderStatus = order.OrderStatus;

            // Save the changes to the database
            await _context.SaveChangesAsync();

            // Return the updated order
            return existingOrder;

        }
        public async Task<int>UpdateOrderInfo(Order order)
        {
            var result = 0;
            var existingOrder = _context.Orders.FirstOrDefault(o => o.ID == order.ID);
            if (existingOrder != null)
            {
                existingOrder.FirstName = order.FirstName;
                existingOrder.LastName = order.LastName;
                existingOrder.Email = order.Email;
                existingOrder.Address = order.Address;
                existingOrder.Address2 = order.Address2;
                existingOrder.Phone = order.Phone;
                existingOrder.Fax = order.Fax;

                result = _context.SaveChanges();
            }

            return result;
        }
        public async Task CreateOrderItems(IEnumerable<OrderItems> orderItems)
        {
            _context.OrderItems.AddRange(orderItems);
            await _context.SaveChangesAsync();
        }
        public async Task<OrderItems> CreateOrderItem(OrderItems orderItem)
        {
            _context.Entry(orderItem).State = EntityState.Added;
            await _context.SaveChangesAsync();

            return orderItem;
        }
        public async Task<Order> GetLatestOrderForUser(string userId)
        {
            return await _context.Orders.AsNoTracking().IgnoreQueryFilters()
                .Where(order => order.UserID == userId)
                .OrderByDescending(order => order.ID)
                .FirstOrDefaultAsync();
        }

        public async Task<Order> GetOrderByOrderId(int Id)
        {
            return await _context.Orders.AsNoTracking().IgnoreQueryFilters()
                .FirstOrDefaultAsync(x => x.ID == Id);
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserId(string userId)
        {
            return await _context.Orders.AsNoTracking().IgnoreQueryFilters()
                .Where(order => order.UserID == userId)
                .ToListAsync();
        }
        
        public async Task<IEnumerable<Order>> GetOrdersByGroup(Guid orderGroup)
        {
            return await _context.Orders.AsNoTracking().IgnoreQueryFilters()
                .Where(order => order.OrderGroup == orderGroup)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrders()
        {
            return await _context.Orders.ToListAsync();
        }

        public async Task<IList<OrderItems>> GetOrderItemsByOrderId(int orderId)
        {
            return await _context.OrderItems.AsNoTracking().IgnoreQueryFilters()
                .Where(orderItems => orderItems.OrderID == orderId )
                .Include(x => x.Order)
                .Include(x => x.Subproduct).ThenInclude(sp => sp.Image)
                .Include(x => x.Subproduct).ThenInclude(sp => sp.Product).ThenInclude(x => x.Flags)
                .ToListAsync();
        }

        public async Task<List<OrderItems>> GetOrderItems()
        {
            return await _context.OrderItems.AsNoTracking().IgnoreQueryFilters()
                 .Include(x => x.Order)
                .Include(x => x.Subproduct).ThenInclude(sp => sp.Image)
                .Include(x => x.Subproduct).ThenInclude(sp => sp.Product)
                .ToListAsync();
        }

        public async Task<List<OrderItems>> GetOrderItemsByUserId(string userId)
        {
            return await _context.OrderItems.AsNoTracking().IgnoreQueryFilters()
                .Include(x => x.Order)
                .Include(x => x.Subproduct).ThenInclude(sp => sp.Image)
                .Include(x => x.Subproduct).ThenInclude(sp => sp.Product)
                .Where(oi => oi.Order.UserID == userId)
                .ToListAsync();
        }

        public async Task<List<OrderItems>> GetOrderItemsByOrderGroup(Guid orderGroup)
        {
            return await _context.OrderItems.AsNoTracking().IgnoreQueryFilters()
                .Include(x => x.Order)
                .Include(x => x.Subproduct).ThenInclude(sp => sp.Image)
                .Include(x => x.Subproduct).ThenInclude(sp => sp.Product)
                .Where(oi => oi.Order.OrderGroup == orderGroup)
                .ToListAsync();
        }

        public async Task<OrderItems> UpdateOrderItems(OrderItems orderItem)
        {
            var updateOrderItem = new OrderItems
            {
                OrderID = orderItem.ID,
                SubproductId = orderItem.SubproductId,
            };
            _context.Entry(orderItem).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return updateOrderItem;
        }

        public async Task DeleteOrder(int id)
        {
            Order order = await GetOrderByOrderId(id);
            _context.Entry(order).State = EntityState.Deleted;
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetOrdersCountAsync()
        {
            return await _context.Orders.CountAsync();
        }
        public async Task<int> GetPendingOrdersCountAsync()
        {
            return await _context.Orders
                .Where(p => p.OrderStatus.Contains("Pending"))
                .CountAsync();
        }
        public async Task<int> GetCanceledOrdersCountAsync()
        {
            return await _context.Orders
                .Where(p => p.OrderStatus.Contains("Cancelled"))
                .CountAsync();
        }
        public async Task<int> GetAcceptsOrdersCountAsync()
        {
            return await _context.Orders
                .Where(p => p.OrderStatus.Contains("Accepted"))
                .CountAsync();
        }
        public async Task<int> GetRejectedOrdersCountAsync()
        {
            return await _context.Orders
                .Where(p => p.OrderStatus.Contains("Rejected"))
                .CountAsync();
        }
        public async Task<int> GetPaidCountAsync()
        {
            return await _context.Orders
                .Where(p => p.PaymentStstus == "Paid")
                .CountAsync();
        }
        public async Task<int> GetUnPaidCountAsync()
        {
            return await _context.Orders
                .Where(p => p.PaymentStstus == "UnPaid")
                .CountAsync();
        }

        public async Task<Order> CancelOrder(int id)
        {
            var existingOrder = await _context.Orders.FindAsync(id);
            if (existingOrder == null)
            {
                throw new Exception("Order not found.");
            }
            existingOrder.OrderStatus = "Cancelled by user";
            _context.Entry(existingOrder).State = EntityState.Modified;
            var x = await _context.SaveChangesAsync();
            return existingOrder;
        }
    }
}
