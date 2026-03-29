using GabriniCosmetics.Areas.Admin.Models.DTOs;
using GabriniCosmetics.Areas.Admin.Models.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GabriniCosmetics.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DealOfTheDaysController : Controller
    {
        private readonly IDealOfTheDaysService _dealService;

        public DealOfTheDaysController(IDealOfTheDaysService dealService)
        {
            _dealService = dealService;
        }

        // GET: DealOfTheDays/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var deals = await _dealService.GetAllDealsAsync(); // Assuming a method to get all deals
            return View(deals);
        }

        // GET: DealOfTheDays/Update/{id}
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var deal = await _dealService.GetDealOfTheDayByIdAsync(id);
            if (deal == null)
            {
                return NotFound(); // Show a not found view or error message
            }

            var dto = new UpdateDealOfTheDaysDTO
            {
                EndTime = deal.EndTime,
                Id = id,
                ExistingImagePath = deal.ImageUpload
            };

            return View(dto);
        }

        // POST: DealOfTheDays/Update/{id}
        [HttpPost]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateDealOfTheDaysDTO dto)
        {
            try
            {
                await _dealService.UpdateDealOfTheDaysAsync(id, dto);
                return Redirect("/Admin/DealOfTheDays");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message); // Add error to the model state
                return View(dto); // Return the view with errors
            }
        }
    }

}
