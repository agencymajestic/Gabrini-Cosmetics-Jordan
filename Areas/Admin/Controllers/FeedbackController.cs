using GabriniCosmetics.Areas.Admin.Models.Interface;
using GabriniCosmetics.Areas.Admin.Models.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GabriniCosmetics.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class FeedbackController : Controller
    {
        private readonly IContactUs _contactUs;

        public FeedbackController(IContactUs contactUs)
        {
            _contactUs = contactUs;
        }
        public async Task<IActionResult> Index()
        {
            var feedback = await _contactUs.GetFeedback();
            return View(feedback);
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] int id)
        {
            var success = await _contactUs.Delete(id);
            if (!success)
            {
                return NotFound();
            }
            return Ok();
        }
    }
}
