using GabriniCosmetics.Areas.Admin.Models.Interface;
using GabriniCosmetics.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;

namespace GabriniCosmetics.Controllers
{
    public class ContactusController : Controller
    {
        private readonly IContactUs _contactUs;
        string culture = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;

        public ContactusController(IContactUs contactUs)
        {
            _contactUs = contactUs; 
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(ContactUs contactUs)
        {
            if (ModelState.IsValid)
            {
                await _contactUs.Create(contactUs);
                ViewBag.IsSuccess = true;
                return RedirectToAction("Index");
            }
            return BadRequest();
        }
    }
}
