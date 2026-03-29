using GabriniCosmetics.Areas.Admin.Models;
using GabriniCosmetics.Areas.Admin.Models.CustomerInfo;
using GabriniCosmetics.Areas.Admin.Models.Interface;
using GabriniCosmetics.Areas.Admin.Models.Services;
using GabriniCosmetics.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace GabriniCosmetics.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrder _order;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderController(IOrder order, IHttpContextAccessor httpContextAccessor)
        {
            _order = order;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IActionResult> Index()
        {
            var orderVM = new OrderVM();
            if (User.Identity.IsAuthenticated)
            {
                var userId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault().Value;

                var orderItems = await _order.GetOrderItemsByUserId(userId);
                var orders = await _order.GetOrdersByUserId(userId);
                //var orders = orderItems.Select(oi => oi.Order).ToList();

                if (orders.IsNullOrEmpty())
                {
                    orders = new List<Order>();
                }

                orderVM.Orders = orders.OrderByDescending(x => x.ID);
                orderVM.OrderItems = orderItems;
            }
            else
            {
                var groupValue = Request.Cookies["_cartgroup"];
                if (groupValue.IsNullOrEmpty())
                {
                    return Redirect("/");
                }

                bool parseResult = Guid.TryParse(groupValue, out Guid cartGroup);

                if (!parseResult)
                {
                    return Redirect("/");
                }

                var orderItems = await _order.GetOrderItemsByOrderGroup(cartGroup);
                var orders = orderItems.Select(oi => oi.Order).ToList();

                if (orders.IsNullOrEmpty())
                {
                    orders = new List<Order>();
                }

                orderVM.Orders = orders.OrderByDescending(x => x.ID);
                orderVM.OrderItems = orderItems;
            }

            return View(orderVM);
        }
        public async Task<IActionResult> Detail(int id)
        {
            var orderDetailVM = new OrderDetailVM();
            orderDetailVM.OrderItems = await _order.GetOrderItemsByOrderId(id);
            orderDetailVM.Tax = 2;
            orderDetailVM.PriceAfterDiscount = orderDetailVM.OrderItems.First().DiscountCode != null ? ((decimal)orderDetailVM.OrderItems.First().TotalPrice - orderDetailVM.Tax) : null;
            orderDetailVM.PriceBeforeDiscount = orderDetailVM.OrderItems.First().DiscountCode != null ? (decimal)((decimal)orderDetailVM.OrderItems.First().TotalPrice - orderDetailVM.Tax + orderDetailVM.OrderItems.First().TotalDiscount) : null;

            //orderDetailVM.OrderItems.First().Order = await _order.GetOrderByOrderId(orderDetailVM.OrderItems.First().OrderID);

            return View(orderDetailVM);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateInfo(Order order)
        {
            var result = await _order.UpdateOrderInfo(order);
            //if (ModelState.IsValid)
            //{
            //    var existingOrder = _dbContext.Orders.FirstOrDefault(o => o.ID == order.ID);
            //    if (existingOrder != null)
            //    {
            //        existingOrder.FirstName = order.FirstName;
            //        existingOrder.LastName = order.LastName;
            //        existingOrder.Email = order.Email;
            //        existingOrder.Address = order.Address;
            //        existingOrder.Address2 = order.Address2;
            //        existingOrder.City = order.City;
            //        existingOrder.Phone = order.Phone;
            //        existingOrder.Fax = order.Fax;

            //        var result = await _order.EditOrder(order.ID, order);
            //        _dbContext.SaveChanges();
            //        return RedirectToAction("SuccessPageOrWherever");
            //    }
            //    ModelState.AddModelError("", "Order not found.");
            //}

            return RedirectToAction("Detail", new { id = order.ID });
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            await _order.CancelOrder(id);
            return Redirect("/Admin");
        }

    }
}
