using GabriniCosmetics.Areas.Admin.Models.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel;

namespace GabriniCosmetics.Areas.Admin.Models.Components
{
    [ViewComponent(Name = "SmallCart")]
    public class SmallCartViewComponent : ViewComponent
    {
        private readonly ICartService _cartRepo;

        public SmallCartViewComponent(ICartService cartRepo)
        {
            _cartRepo = cartRepo;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            ShoppingCart cart;
            if (!User.Identity.IsAuthenticated)
            {
                Guid cartGroup;
                var groupValue = HttpContext.Request.Cookies["_cartgroup"];
                if (groupValue.IsNullOrEmpty())
                {
                    var options = new CookieOptions()
                    {
                        HttpOnly = true,
                        IsEssential = true,
                        SameSite = SameSiteMode.Strict,
                        Secure = HttpContext.Request.IsHttps,
                        Expires = DateTime.Now.AddDays(30),
                        MaxAge = TimeSpan.FromDays(30),
                        Path = "/",
                        Domain = null
                    };

                    cartGroup = Guid.NewGuid();

                    HttpContext.Response.Cookies.Append("_cartgroup", cartGroup.ToString(), options);
                }
                else
                {
                    bool parseResult = Guid.TryParse(groupValue, out cartGroup);

                    if (!parseResult)
                    {
                        return View(new ShoppingCart());
                    }
                }

                cart = await _cartRepo.GetUnknownCart(cartGroup);

            }
            else
            {
                cart = await _cartRepo.GetUserCart();
            }

            return View(cart);
        }
    }
}
