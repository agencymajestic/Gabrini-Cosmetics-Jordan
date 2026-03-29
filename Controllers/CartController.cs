using GabriniCosmetics.Areas.Admin.Models;
using GabriniCosmetics.Areas.Admin.Models.Interface;
using GabriniCosmetics.Areas.Admin.Models.Services;
using GabriniCosmetics.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics.Metrics;
using System.Drawing;

namespace GabriniCosmetics.Controllers
{
    //[Authorize]
    public class CartController : Controller
    {
        private readonly ICartService _cartRepo;
        private readonly GabriniCosmeticsContext _db;


        public CartController(ICartService cartRepo, GabriniCosmeticsContext gabriniCosmeticsContext)
        {
            _cartRepo = cartRepo;
            _db = gabriniCosmeticsContext;
        }
        //[Authorize]
        public async Task<IActionResult> AddItem(int subproductId, int qty = 1, int redirect = 1)
        {
            int cartCount;
            if (!User.Identity.IsAuthenticated)
            {
                Guid cartGroup;
                var groupValue = Request.Cookies["_cartgroup"];
                if (groupValue.IsNullOrEmpty())
                {
                    var options = new CookieOptions()
                    {
                        HttpOnly = true,
                        IsEssential = true,
                        SameSite = SameSiteMode.Strict,
                        Secure = Request.IsHttps,
                        Expires = DateTime.Now.AddDays(30),
                        MaxAge = TimeSpan.FromDays(30),
                        Path = "/",
                        Domain = null
                    };

                    cartGroup = Guid.NewGuid();

                    Response.Cookies.Append("_cartgroup", cartGroup.ToString(), options);
                }
                else
                {
                    bool parseResult = Guid.TryParse(groupValue, out cartGroup);

                    if (!parseResult)
                    {
                        return Redirect("/");
                    }
                }

                cartCount = await _cartRepo.AddUnknownUserItem(subproductId, cartGroup);
            }
            else
            {
                cartCount = await _cartRepo.AddItem(subproductId);
            }
            if (redirect == 0)
                return Ok(cartCount);
            string returnUrl = Request.Headers["Referer"].ToString() ?? "/";
            return Redirect(returnUrl);
        }
        //[Authorize]
        public async Task<IActionResult> AddItemWithQtyAndImage(int subproductId, int qty)
        {
            if (qty != 0)
            {
                if(!User.Identity.IsAuthenticated)
                {
                    Guid cartGroup;
                    var groupValue = Request.Cookies["_cartgroup"];
                    if (groupValue.IsNullOrEmpty())
                    {
                        var options = new CookieOptions()
                        {
                            HttpOnly = true,
                            IsEssential = true,
                            SameSite = SameSiteMode.Strict,
                            Secure = Request.IsHttps,
                            Expires = DateTime.Now.AddDays(30),
                            MaxAge = TimeSpan.FromDays(30),
                            Path = "/",
                            Domain = null
                        };

                        cartGroup = Guid.NewGuid();

                        Response.Cookies.Append("_cartgroup", cartGroup.ToString(), options);
                    }
                    else
                    {
                        bool parseResult = Guid.TryParse(groupValue, out cartGroup);

                        if (!parseResult)
                        {
                            return Redirect("/");
                        }
                    }

                    await _cartRepo.AddUnknownUserItem(subproductId, qty, cartGroup);
                }
                else
                {
                    var cartCount = await _cartRepo.AddItem(subproductId, qty);
                }

            }
            string returnUrl = Request.Headers["Referer"].ToString() ?? "/";
            return Redirect(returnUrl);
        }
        public async Task<IActionResult> RemoveItem(int subproductId)
        {

            if (!User.Identity.IsAuthenticated)
            {
                Guid cartGroup;
                var groupValue = Request.Cookies["_cartgroup"];
                if (groupValue.IsNullOrEmpty())
                {
                    var options = new CookieOptions()
                    {
                        HttpOnly = true,
                        IsEssential = true,
                        SameSite = SameSiteMode.Strict,
                        Secure = Request.IsHttps,
                        Expires = DateTime.Now.AddDays(30),
                        MaxAge = TimeSpan.FromDays(30),
                        Path = "/",
                        Domain = null
                    };

                    cartGroup = Guid.NewGuid();

                    Response.Cookies.Append("_cartgroup", cartGroup.ToString(), options);
                }
                else
                {
                    bool parseResult = Guid.TryParse(groupValue, out cartGroup);

                    if (!parseResult)
                    {
                        return Redirect("/");
                    }
                }

                await _cartRepo.RemoveUnknownCartItem(subproductId, cartGroup);
            }
            else
            {
                await _cartRepo.RemoveItem(subproductId);
            }

            string returnUrl = Request.Headers["Referer"].ToString() ?? "/";
            return Redirect(returnUrl);
        }
        //[Authorize]
        public async Task<IActionResult> index()
        {
            ShoppingCart cart;
            if (!User.Identity.IsAuthenticated)
            {
                Guid cartGroup;
                var groupValue = Request.Cookies["_cartgroup"];
                if (groupValue.IsNullOrEmpty())
                {
                    var options = new CookieOptions()
                    {
                        HttpOnly = true,
                        IsEssential = true,
                        SameSite = SameSiteMode.Strict,
                        Secure = Request.IsHttps,
                        Expires = DateTime.Now.AddDays(30),
                        MaxAge = TimeSpan.FromDays(30),
                        Path = "/",
                        Domain = null
                    };

                    cartGroup = Guid.NewGuid();

                    Response.Cookies.Append("_cartgroup", cartGroup.ToString(), options);
                }
                else
                {
                    bool parseResult = Guid.TryParse(groupValue, out cartGroup);

                    if (!parseResult)
                    {
                        return Redirect("/");
                    }
                }

                cart = await _cartRepo.GetUnknownCart(cartGroup);
            }
            else
            {
                cart = await _cartRepo.GetUserCart();
            }


            //foreach (var item in cart.CartDetails)
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
            return View(cart);
        }
        public async Task<IActionResult> GetTotalItemInCart()
        {

            int count;
            if (!User.Identity.IsAuthenticated)
            {
                Guid cartGroup;
                var groupValue = Request.Cookies["_cartgroup"];
                if (groupValue.IsNullOrEmpty())
                {
                    var options = new CookieOptions()
                    {
                        HttpOnly = true,
                        IsEssential = true,
                        SameSite = SameSiteMode.Strict,
                        Secure = Request.IsHttps,
                        Expires = DateTime.Now.AddDays(30),
                        MaxAge = TimeSpan.FromDays(30),
                        Path = "/",
                        Domain = null
                    };

                    cartGroup = Guid.NewGuid();

                    Response.Cookies.Append("_cartgroup", cartGroup.ToString(), options);
                }
                else
                {
                    bool parseResult = Guid.TryParse(groupValue, out cartGroup);

                    if (!parseResult)
                    {
                        return Redirect("/");
                    }
                }

                count = await _cartRepo.GetUnknownCartItemCount(cartGroup);
            }
            else
            {
                count = await _cartRepo.GetCartItemCount();
            }


            return Ok(count);
        }
        [HttpPost]
        public async Task<IActionResult> AddSelectedToCart([FromBody] SelectedItemsDto selectedItemsDto)
        {
            var selectedItems = selectedItemsDto.SelectedItems;
            if (!User.Identity.IsAuthenticated)
            {
                Guid cartGroup;
                var groupValue = Request.Cookies["_cartgroup"];
                if (groupValue.IsNullOrEmpty())
                {
                    var options = new CookieOptions()
                    {
                        HttpOnly = true,
                        IsEssential = true,
                        SameSite = SameSiteMode.Strict,
                        Secure = Request.IsHttps,
                        Expires = DateTime.Now.AddDays(30),
                        MaxAge = TimeSpan.FromDays(30),
                        Path = "/",
                        Domain = null
                    };

                    cartGroup = Guid.NewGuid();

                    Response.Cookies.Append("_cartgroup", cartGroup.ToString(), options);
                }
                else
                {
                    bool parseResult = Guid.TryParse(groupValue, out cartGroup);

                    if (!parseResult)
                    {
                        return Redirect("/");
                    }
                }
                foreach (var itemId in selectedItems)
                {
                    var wishListItem = await _db.WishlistDetail.Include(w => w.Subproduct).ThenInclude(sp => sp.Product).FirstOrDefaultAsync(w => w.Id == itemId);
                    if (wishListItem != null)
                    {
                        await _cartRepo.AddUnknownUserItem(wishListItem.SubproductId, wishListItem.Quantity, cartGroup);
                    }
                }
            }
            else
            {
                foreach (var itemId in selectedItems)
                {
                    var wishListItem = await _db.WishlistDetail.Include(w => w.Subproduct).ThenInclude(sp => sp.Product).FirstOrDefaultAsync(w => w.Id == itemId);
                    if (wishListItem != null)
                    { 
                        await _cartRepo.AddItem(wishListItem.SubproductId, wishListItem.Quantity);
                    }
                }
            }
            return Redirect("/Cart");
        }
    }
    public class SelectedItemsDto
    {
        public int[] SelectedItems { get; set; }
    }
}
