using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using GabriniCosmetics.Areas.Admin.Models.Interface;
using GabriniCosmetics.Areas.Admin.Models;
using Microsoft.IdentityModel.Tokens;

//[Authorize]
public class WishlistController : Controller
{
    private readonly IWishlistService _wishlistService;

    public WishlistController(IWishlistService wishlistService)
    {
        _wishlistService = wishlistService;
    }
    //[Authorize]
    public async Task<IActionResult> AddItem(int subproductId, int qty = 1, int redirect = 1)
    {
        int wishlistCount;
        if (!User.Identity.IsAuthenticated)
        {
            Guid wishListGroup;
            var groupValue = Request.Cookies["_wishListGroup"];
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

                wishListGroup = Guid.NewGuid();

                Response.Cookies.Append("_wishListGroup", wishListGroup.ToString(), options);
            }
            else
            {
                bool parseResult = Guid.TryParse(groupValue, out wishListGroup);

                if (!parseResult)
                {
                    return Redirect("/");
                }
            }

            wishlistCount = await _wishlistService.AddUnknownUserItem(subproductId, wishListGroup);
        }
        else
        {
            wishlistCount = await _wishlistService.AddItem(subproductId); ;
        }
        if (redirect == 0)
            return Ok(wishlistCount);

        string returnUrl = Request.Headers["Referer"].ToString() ?? "/";
        return Redirect(returnUrl);
    }

    //[Authorize]
    public async Task<IActionResult> AddItemWithQtyAndImage(int subproductId, int qty)
    {
        if (qty != 0)
        {
            if (!User.Identity.IsAuthenticated)
            {
                Guid wishListGroup;
                var groupValue = Request.Cookies["_wishListGroup"];
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

                    wishListGroup = Guid.NewGuid();

                    Response.Cookies.Append("_wishListGroup", wishListGroup.ToString(), options);
                }
                else
                {
                    bool parseResult = Guid.TryParse(groupValue, out wishListGroup);

                    if (!parseResult)
                    {
                        return Redirect("/");
                    }
                }

                await _wishlistService.AddUnknownUserItem(subproductId, qty, wishListGroup);
            }
            else
            {
                await _wishlistService.AddItem(subproductId, qty);
            }

        }
        string returnUrl = Request.Headers["Referer"].ToString() ?? "/";
        return Redirect(returnUrl);
    }

    public async Task<IActionResult> RemoveItem(int subproductId)
    {
        if (!User.Identity.IsAuthenticated)
        {
            Guid wishListGroup;
            var groupValue = Request.Cookies["_wishListGroup"];
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

                wishListGroup = Guid.NewGuid();

                Response.Cookies.Append("_wishListGroup", wishListGroup.ToString(), options);
            }
            else
            {
                bool parseResult = Guid.TryParse(groupValue, out wishListGroup);

                if (!parseResult)
                {
                    return Redirect("/");
                }
            }

            await _wishlistService.RemoveUnknownWishListItem(subproductId, wishListGroup);
        }
        else
        {
            await _wishlistService.RemoveItem(subproductId);
        }

        string returnUrl = Request.Headers["Referer"].ToString() ?? "/";
        return Redirect(returnUrl);
    }
    //[Authorize]
    public async Task<IActionResult> Index()
    {
        Wishlist wishList;
        if (!User.Identity.IsAuthenticated)
        {
            Guid wishListGroup;
            var groupValue = Request.Cookies["_wishListGroup"];
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

                wishListGroup = Guid.NewGuid();

                Response.Cookies.Append("_wishListGroup", wishListGroup.ToString(), options);
            }
            else
            {
                bool parseResult = Guid.TryParse(groupValue, out wishListGroup);

                if (!parseResult)
                {
                    return Redirect("/");
                }
            }

            wishList = await _wishlistService.GetUnknownWishList(wishListGroup);
        }
        else
        {
            wishList = await _wishlistService.GetUserWishlist();
        }


        return View(wishList);
    }

    public async Task<IActionResult> GetTotalItemInWishlist()
    {
        int count;
        if (!User.Identity.IsAuthenticated)
        {
            Guid wishListGroup;
            var groupValue = Request.Cookies["_wishListGroup"];
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

                wishListGroup = Guid.NewGuid();

                Response.Cookies.Append("_wishListGroup", wishListGroup.ToString(), options);
            }
            else
            {
                bool parseResult = Guid.TryParse(groupValue, out wishListGroup);

                if (!parseResult)
                {
                    return Redirect("/");
                }
            }

            count = await _wishlistService.GetUnknownWishListItemCount(wishListGroup);
        }
        else
        {
            count = await _wishlistService.GetWishlistItemCount();
        }

        return Ok(count);
    }
    
    
    
    
    // Example action to display wishlist items by user
    public async Task<IActionResult> GetWishlistItemsByUser(string userId)
    {
        var wishlistItems = await _wishlistService.GetWishlistProductByUserId(userId);
        return View(wishlistItems);
    }
}
