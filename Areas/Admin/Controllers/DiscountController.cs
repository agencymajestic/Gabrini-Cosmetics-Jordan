using GabriniCosmetics.Areas.Admin.Models.Interface;
using GabriniCosmetics.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using GabriniCosmetics.Areas.Admin.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using GabriniCosmetics.Areas.Admin.Models.Services;
using GabriniCosmetics.Data;
using Microsoft.AspNetCore.Identity;

namespace GabriniCosmetics.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = "Admin")]
    public class DiscountController : Controller
    {
        private readonly IDiscountService _discountService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICartService _shop;

        public DiscountController(IDiscountService discountService, ICartService shop, UserManager<ApplicationUser> userManager)
        {
            _discountService = discountService;
            _shop = shop;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Discount> discounts = await _discountService.GetAllDiscountsAsync();
            return View(discounts);
        }

        public IActionResult Create()
        {
            return View();
        }

        // POST: Discount/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DiscountViewModel model)
        {
            if (ModelState.IsValid)
            {
                var discount = new Discount
                {
                    Code = model.Code,
                    Percentage = model.Percentage,
                    ValidFrom = model.ValidFrom,
                    ValidTo = model.ValidTo
                };

                await _discountService.AddDiscountAsync(discount);
                return Redirect("/Admin/Discount");
            }

            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            // Fetch the discount by ID
            var discount = await _discountService.GetDiscountById(id);
            if (discount == null)
            {
                return NotFound();
            }

            // Map discount to EditDiscountDTO
            var model = new Discount
            {
                 Id = discount.Id,
                Code = discount.Code,
                Percentage = discount.Percentage,
                ValidFrom = discount.ValidFrom,
                ValidTo = discount.ValidTo
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Discount model)
        {
            if (ModelState.IsValid)
            {
                // Update the discount
                await _discountService.UpdateDiscount(model);
                return Redirect("/Admin/Discount");
            }

            return View(model); // Return the same view with validation errors if the model is invalid
        }


        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] int id)
        {
            var success = await _discountService.Delete(id);
            if (!success)
            {
                return NotFound();
            }
            return Ok();
        }

        public async Task<IActionResult> ApplyDiscount([FromBody] DiscountRequest request)
        {
            //var result = await _discountService.ApplyDiscountAsync(request.Code, request.OriginalPrice);
            //if (result.IsSuccess)
            //{
            //    return Ok(new { DiscountedPrice = result.DiscountedPrice });
            //}
            //return BadRequest("Invalid discount code");

            ApplicationUser user;
            Guid? cartGroup = null;

            // Identify user or guest
            if (User.Identity.IsAuthenticated)
            {
                user = await _userManager.GetUserAsync(User);
            }
            else
            {
                user = await _userManager.FindByEmailAsync("unknown@GabriniCosmetics.com");
                var groupValue = Request.Cookies["_cartgroup"];

                if (string.IsNullOrEmpty(groupValue) || !Guid.TryParse(groupValue, out Guid group))
                {
                    return BadRequest("Invalid cart group.");
                }

                cartGroup = group;
            }

            ShoppingCart shoppingCart;
            // Get shopping cart
            if (User.Identity.IsAuthenticated)
            {
                shoppingCart = await _shop.GetCart(user.Id);
            }
            else
            {
                shoppingCart = await _shop.GetUnknownUserCart(cartGroup.Value);
            }

            if (shoppingCart is null || !shoppingCart.CartDetails.Any())
            {
                throw new Exception("Cart Not Found");
            }

            // Calculate original price
            var originalPrice = shoppingCart.CartDetails.Sum(item =>
                item.Subproduct.Product.Flags.Any(f => f.FlagType == "Sale")
                    ? item.Subproduct.Product.PriceAfterDiscount * item.Quantity
                    : item.Subproduct.Product.Price * item.Quantity
            );

            // Apply discount
            var result = await _discountService.ApplyDiscountAsync(request.Code, (decimal)originalPrice);
            if (!result.IsSuccess)
            {
                return BadRequest("Invalid discount code");
            }

            // Estimate delivery tax (simplified logic)
            decimal tax = 0;

            if (request.CityName != null)
            {
                tax = 2;
            }
            else if (request.TaxValue != null)
            {
                tax = 2;
            }

            decimal totalAfterDiscount = result.DiscountedPrice + tax;

            return Ok(new
            {
                OriginalPrice = originalPrice,
                DiscountedPrice = result.DiscountedPrice,
                DeliveryTax = tax,
                TotalPrice = totalAfterDiscount
            });
        }

        //}
        //public async Task<IActionResult> ApplyDiscount([FromBody] DiscountRequest request)
        //{
        //}
    }

    public class DiscountRequest
    {
        public string Code { get; set; }
        public string CityName { get; set; } // from address or city
        public decimal? TaxValue { get; set; } // fallback tax
    }

}
