using GabriniCosmetics.Areas.Admin.Models;
using GabriniCosmetics.Areas.Admin.Models.Interface;
using GabriniCosmetics.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GabriniCosmetics.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        private readonly IOrder _orderService;

        public OrderController(IOrder order)
        {
            _orderService = order;
        }
        public async Task<IActionResult> Index()
        {
            var listOrder = await _orderService.GetOrders();

            return View(listOrder.OrderByDescending(x => x.ID));
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, string paymentStatus, string orderStatus)
        {
            var order = await _orderService.GetOrderByOrderId(id);
            if (order == null)
            {
                return NotFound();
            }

            // Update order properties
            order.OrderStatus = orderStatus;
            order.PaymentStstus = paymentStatus;

            await _orderService.EditOrder(id, order);
            return Redirect("/Admin");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var orderDetailVM = new OrderDetailVM();
            orderDetailVM.OrderItems = await _orderService.GetOrderItemsByOrderId(id);

            orderDetailVM.OrderItems.First().Order = await _orderService.GetOrderByOrderId(orderDetailVM.OrderItems.First().OrderID);
            orderDetailVM.Tax = 2;
            orderDetailVM.PriceAfterDiscount = (decimal)orderDetailVM.OrderItems.First().TotalPrice - orderDetailVM.Tax;
            orderDetailVM.PriceBeforeDiscount = (orderDetailVM.OrderItems.First().DiscountCode != null) ? (decimal)((decimal)orderDetailVM.OrderItems.First().TotalPrice - orderDetailVM.Tax + orderDetailVM.OrderItems.First().TotalDiscount) : null;
            return View(orderDetailVM);
        }
        public async Task<IActionResult> Delete(int id)
        {
            await _orderService.DeleteOrder(id);
            return RedirectToAction("Index");
        }
    }
}
