using Microsoft.AspNetCore.Mvc;

namespace GabriniCosmetics.Controllers
{
    public class FlipbookController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
