using GabriniCosmetics.Areas.Admin.Models.Interface;
using GabriniCosmetics.Areas.Admin.Models;
using GabriniCosmetics.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using GabriniCosmetics.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using GabriniCosmetics.Models.Services;
using GabriniCosmetics.Areas.Admin.Models.Services;
using Microsoft.IdentityModel.Tokens;
using GabriniCosmetics.Areas.Admin.Models.CustomerInfo;

namespace GabriniCosmetics.Controllers
{
    //[Authorize]
    public class CheckoutController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICartService _shop;
        private readonly IOrder _order;
        private readonly IProduct _product;
        private readonly IAddressService _addresses;
        private readonly IDiscountService _discountService;
        private readonly GabriniCosmeticsContext _context;

        public CheckoutController(UserManager<ApplicationUser> userManager, ICartService shop, IOrder order,
            IProduct product, IAddressService addresses, GabriniCosmeticsContext context, IDiscountService discountService)
        {
            _userManager = userManager;
            _shop = shop;
            _order = order;
            _product = product;
            _addresses = addresses;
            _context = context;
            _discountService = discountService;
        }
        //[Authorize]
        public async Task<IActionResult> Completed(int order)
        {
            var orderById = await _order.GetOrderByOrderId(order);
            return View(orderById);
        }

        public async Task<IActionResult> Index()
        {
            var checkoutVM = new CheckoutVM();
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.isAuth = "True";
                var user = await _userManager.GetUserAsync(User);
                checkoutVM.Addresses = await _addresses.GetAddressByUserIdAsync(user.Id);
                checkoutVM.ShoppingCart = await _shop.GetUserCart();

                //foreach (var item in checkoutVM.ShoppingCart.CartDetails)
                //{
                //    var subCategory = await _subCategoryService.GetSubcategoryById(item.Subproduct.Product.SubcategoryId);
                //    var category = await _categoryService.GetCategoryById(subCategory.CategoryId);
                //    item.Subproduct.Product.Subcategory = new Subcategory
                //    {
                //        Id = subCategory.Id,
                //        NameAr = subCategory.NameAr,
                //        NameEn = subCategory.NameEn,
                //        CategoryId = category.Id,
                //        Category = new Category
                //        {
                //            NameEn = category.NameEn,
                //            NameAr = category.NameAr,
                //            ImageUpload = category.ImageUpload,
                //        }
                //    };
                //}
            }
            else
            {
                ViewBag.isAuth = "False";
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

                checkoutVM.Addresses = new List<Address>();
                checkoutVM.ShoppingCart = await _shop.GetUnknownCart(cartGroup);
                if (checkoutVM.ShoppingCart.CartDetails.Count == 0)
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            return View(checkoutVM);
        }

        [HttpPost]
        //[Authorize]

        public async Task<IActionResult> Index(CheckoutVM checkoutInput)
        {
            Guid? cartGroup = null;
            ApplicationUser user;
            if (User.Identity.IsAuthenticated)
            {
                user = await _userManager.GetUserAsync(User);
            }
            else
            {
                user = await _userManager.FindByEmailAsync("unknown@GabriniCosmetics.com");
                var groupValue = Request.Cookies["_cartgroup"];
                if (groupValue.IsNullOrEmpty())
                {
                    return Redirect("/");
                }

                bool parseResult = Guid.TryParse(groupValue, out Guid group);

                if (!parseResult)
                {
                    return Redirect("/");
                }

                cartGroup = group;
            }

            double? totalPrice = checkoutInput.TotalPrice;
            if (checkoutInput.Order.FullLocation != "0")
            {
                var getFullAddress = await _addresses.GetAddressByIdAsync(Convert.ToInt32(checkoutInput.Order.FullLocation));
                Order order = new Order
                {
                    UserID = user.Id,
                    FirstName = getFullAddress.FirstName,
                    LastName = getFullAddress.LastName,
                    Address = getFullAddress.Address1,
                    Address2 = getFullAddress.Address2,
                    State = getFullAddress.City,
                    City = getFullAddress.City,
                    Country = getFullAddress.Country,
                    Email = getFullAddress.Email,
                    Fax = getFullAddress.FaxNumber,
                    Phone = getFullAddress.PhoneNumber,
                    Zip = checkoutInput.Order.Zip,
                    Timestamp =  checkoutInput.Order.Timestamp,
                    OrderStatus = checkoutInput.Order.OrderStatus,
                    PaymentMethod = checkoutInput.Order.PaymentMethod,
                    PaymentStstus = checkoutInput.Order.PaymentStstus,
                    OrderGroup = cartGroup.HasValue ? cartGroup.Value : null
                };

                await _context.Orders.AddAsync(order);
                await _context.SaveChangesAsync();

                //var createdOrder = await _order.CreateOrder(order);
                //var latestOrder = await _order.GetLatestOrderForUser(user.Id);
                ShoppingCart shoppingCart;
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



                IEnumerable<CartDetail> cartItems = shoppingCart.CartDetails;
                IList<OrderItems> orderItems = new List<OrderItems>();
                decimal total = 0;

                foreach (var cartItem in cartItems)
                {
                    OrderItems orderItem = new OrderItems
                    {
                        OrderID = order.ID,
                        SubproductId = cartItem.SubproductId,
                        Quantity = cartItem.Quantity,
                        ImageProduct = cartItem.Image,
                        //DiscountCode = result.DiscountedPrice == 0 ? null : checkoutInput.DiscountCode
                    };

                    orderItems.Add(orderItem);
                    var product = await _product.GetProductById(cartItem.Subproduct.ProductId);
                    if (product.IsSale)
                    {
                        total += Convert.ToDecimal(product.PriceAfterDiscount * cartItem.Quantity);
                    }
                    else
                    {
                        total += Convert.ToDecimal(product.Price * cartItem.Quantity);
                    }


                }

                var result = await _discountService.ApplyDiscountAsync(checkoutInput.DiscountCode, total);
                if (result.IsSuccess)
                {
                    var totaldiscount = total - result.DiscountedPrice;
                    total = total - totaldiscount;
                    foreach (var item in orderItems)
                    {
                        item.DiscountCode = result.DiscountedPrice == 0 ? null : checkoutInput.DiscountCode;
                        item.TotalDiscount = (totaldiscount == 0) ? 0 : totaldiscount;
                        order.TotalDiscount = (totaldiscount == 0) ? 0 : totaldiscount;
                    }
                }
                if (getFullAddress != null)
                {
                    if (true)
                    {
                        total = total + 2;
                    }
                    else
                    {
                        total = total + 3;
                    }
                }
                else
                {
                    if (true)
                    {
                        total = total + 2;
                    }
                    else
                    {
                        total = total + 3;
                    }
                }

                    orderItems[0].TotalPrice = Convert.ToDouble(total);
                //double finalCost = total * 1.1;                
                await _order.CreateOrderItems(orderItems);
                await _shop.RemoveCartProducts(cartItems);

                return RedirectToAction("Completed", "Checkout", new { order = order.ID });
            }
            else if (checkoutInput.Order.FirstName != null && checkoutInput.Order.LastName != null && checkoutInput.Order.Address != null && checkoutInput.Order.City != null && checkoutInput.Order.Email != null && checkoutInput.Order.Phone != null)
            {
                var address = new Address
                {
                    Address1 = checkoutInput.Order.Address,
                    Address2 = checkoutInput.Order.Address2,
                    City = (checkoutInput.Order.City == null || checkoutInput.Order.City == string.Empty) ? checkoutInput.Order.State : checkoutInput.Order.City,
                    Country = checkoutInput.Order.Country,
                    Email = checkoutInput.Order.Email,
                    FirstName = checkoutInput.Order.FirstName,
                    LastName = checkoutInput.Order.LastName,
                    PhoneNumber = checkoutInput.Order.Phone,
                    FaxNumber = checkoutInput.Order.Fax,
                    UserId = user.Id
                };
                await _addresses.CreateAddressAsync(address);
                Order order = new Order
                {
                    UserID = user.Id,
                    FirstName = checkoutInput.Order.FirstName,
                    LastName = checkoutInput.Order.LastName,
                    Address = checkoutInput.Order.Address,
                    Address2 = checkoutInput.Order.Address2,
                    State = checkoutInput.Order.City,
                    City = checkoutInput.Order.City,
                    Country = checkoutInput.Order.Country,
                    Email = checkoutInput.Order.Email,
                    Fax = checkoutInput.Order.Fax,
                    Phone = checkoutInput.Order.Phone,
                    Zip = checkoutInput.Order.Zip,
                    Timestamp = checkoutInput.Order.Timestamp,
                    OrderStatus = checkoutInput.Order.OrderStatus,
                    PaymentMethod = checkoutInput.Order.PaymentMethod,
                    PaymentStstus = checkoutInput.Order.PaymentStstus,
                    OrderGroup = cartGroup.HasValue ? cartGroup.Value : null
                };

                await _context.Orders.AddAsync(order);
                await _context.SaveChangesAsync();

                //var createdOrder = await _order.CreateOrder(order);
                //var latestOrder = await _order.GetLatestOrderForUser(user.Id);

                ShoppingCart shoppingCart;
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

                IEnumerable<CartDetail> cartItems = shoppingCart.CartDetails;
                IList<OrderItems> orderItems = new List<OrderItems>();
                decimal total = 0;

                foreach (var cartItem in cartItems)
                {
                    OrderItems orderItem = new OrderItems
                    {
                        OrderID = order.ID,
                        SubproductId = cartItem.SubproductId,
                        Quantity = cartItem.Quantity,
                        ImageProduct = cartItem.Image,
                        //DiscountCode = result.DiscountedPrice == 0 ? null : checkoutInput.DiscountCode
                    };

                    orderItems.Add(orderItem);
                    var product = await _product.GetProductById(cartItem.Subproduct.ProductId);
                    if (product.IsSale)
                    {
                        total += Convert.ToDecimal(product.PriceAfterDiscount * cartItem.Quantity);
                    }
                    else
                    {
                        total += Convert.ToDecimal(product.Price * cartItem.Quantity);
                    }
                }

                var result = await _discountService.ApplyDiscountAsync(checkoutInput.DiscountCode, total);
                if (result.IsSuccess)
                {
                    var totaldiscount = total - result.DiscountedPrice;
                    total = total - totaldiscount;
                    foreach (var item in orderItems)
                    {
                        item.DiscountCode = result.DiscountedPrice == 0 ? null : checkoutInput.DiscountCode;
                        item.TotalDiscount = (totaldiscount == 0) ? 0 : totaldiscount;
                        order.TotalDiscount = (totaldiscount == 0) ? 0 : totaldiscount;
                    }
                }
                if (true)
                {
                    total = total + 2;
                }
                else
                {
                    total = total + 3;
                }
                orderItems[0].TotalPrice = Convert.ToDouble(total);
                //double finalCost = total * 1.1;                
                await _order.CreateOrderItems(orderItems);
                await _shop.RemoveCartProducts(cartItems);

                return RedirectToAction("Completed", "Checkout", new { order = order.ID });
            }
            return View(checkoutInput);
        }

        [HttpPost]
        public async Task<IActionResult> StateChanged([FromBody] StateChanged stateChanged)
        {
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
            var total = shoppingCart.CartDetails.Sum(item =>
                item.Subproduct.Product.Flags.Any(f => f.FlagType == "Sale")
                    ? item.Subproduct.Product.PriceAfterDiscount * item.Quantity
                    : item.Subproduct.Product.Price * item.Quantity
            );
            double priceAfterStateChanged = 0;
            if (stateChanged.StateId == 2 || true)
            {
                priceAfterStateChanged = (double)(total + 2);
            }
            else if (stateChanged.StateId == 0)
            {
                priceAfterStateChanged = (double)total;
            }
            else if (false)
            {
                priceAfterStateChanged = (double)(total + 3);
            }
            return Ok(priceAfterStateChanged);
        }
    }
}
