using GabriniCosmetics.Areas.Admin.Models.DTOs;
using GabriniCosmetics.Areas.Admin.Models.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GabriniCosmetics.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SocialMediaController : Controller
    {

        private readonly ISocialMediaService _service;
        public SocialMediaController(ISocialMediaService service)
        {
            _service = service;
        }
        public async Task<IActionResult> Index()
        {

            return View(await _service.GetAll());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SocialMediaForm socialMedia)
        {
            if (ModelState.IsValid)
            {
                await _service.Add(socialMedia);
                return Redirect("/Admin/SocialMedia");
            }

            return View(socialMedia);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var socialMedia = await _service.Get(id);
            return View(socialMedia);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SocialMediaForm socialMedia)
        {
            await _service.Edit(socialMedia);
            return Redirect("/Admin/SocialMedia");
        }
        public async Task<IActionResult> Delete([FromBody] int id)
        {
            await _service.Delete(id);
            return Redirect("/Admin/SocialMedia");
        }
    }
}