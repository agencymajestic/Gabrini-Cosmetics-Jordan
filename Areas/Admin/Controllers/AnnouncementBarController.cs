using GabriniCosmetics.Areas.Admin.Models;
using GabriniCosmetics.Areas.Admin.Models.DTOs;
using GabriniCosmetics.Areas.Admin.Models.Interface;
using GabriniCosmetics.Areas.Admin.Models.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GabriniCosmetics.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AnnouncementBarController : Controller
    {
        private readonly IAnnouncementBar _announcementBarService;

        public AnnouncementBarController(IAnnouncementBar announcementBarService)
        {
            _announcementBarService = announcementBarService;
        }
        public async Task<IActionResult> Index()
        {
            var announcementBars = await _announcementBarService.GetAllAsync();
            return View(announcementBars);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var announcementBar = await _announcementBarService.GetByIdAsync(id);

            if (announcementBar == null)
            {
                return NotFound();
            }

            return View(announcementBar);
        }

        // POST: Subcategories/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AnnouncementBar announcementBar)
        {
            var updatedAnnouncementBar = await _announcementBarService.UpdateAsync(announcementBar);
            if (updatedAnnouncementBar == null)
            {
                return NotFound();
            }

            return Redirect("/Admin/AnnouncementBar");
        }
    }
}
