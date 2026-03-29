using Microsoft.AspNetCore.Mvc;
using GabriniCosmetics.Areas.Admin.Models.DTOs;
using GabriniCosmetics.Areas.Admin.Models.Interface;
using Microsoft.AspNetCore.Authorization;

namespace GabriniCosmetics.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SliderAdController : Controller
    {
        private readonly ISliderAdService _sliderAdService;

        public SliderAdController(ISliderAdService sliderAdService)
        {
            _sliderAdService = sliderAdService;
        }

        // GET: /Admin/SliderAd
        public async Task<IActionResult> Index()
        {
            var sliderAds = await _sliderAdService.GetAllSliderAds();
            return View(sliderAds);
        }

        // GET: /Admin/SliderAd/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Admin/SliderAd/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateSliderAdDTO createSliderAdDto)
        {
            if (ModelState.IsValid)
            {
                await _sliderAdService.CreateSliderAd(createSliderAdDto);
                return Redirect("/Admin/SliderAd");
            }

            return View(createSliderAdDto);
        }

        // GET: /Admin/SliderAd/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var sliderAd = await _sliderAdService.GetSliderAdById(id);
            if (sliderAd == null)
            {
                return NotFound();
            }

            var updateSliderAdDto = new UpdateSliderAdDTO
            {
                Id = sliderAd.Id,
                ExistingImagePath = sliderAd.ImageUrl,
                Link = sliderAd.Link,
            };

            return View(updateSliderAdDto);
        }

        // POST: /Admin/SliderAd/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateSliderAdDTO updateSliderAdDto)
        {
            // Retrieve the existing image path if no new image is uploaded
            if (updateSliderAdDto.ImageUpload == null && string.IsNullOrEmpty(updateSliderAdDto.ExistingImagePath))
            {
                var sliderAdDto = await _sliderAdService.GetSliderAdById(updateSliderAdDto.Id);
                if (sliderAdDto != null)
                {
                    updateSliderAdDto.ExistingImagePath = sliderAdDto.ImageUrl;
                }
            }
            
            await _sliderAdService.UpdateSliderAd(updateSliderAdDto);
            return Redirect("/Admin/SliderAd");
        }

        // POST: /Admin/SliderAd/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] int id)
        {
            var success = await _sliderAdService.DeleteSliderAd(id);
            if (success)
            {
                return Redirect("/Admin/SliderAd");
            }

            return BadRequest();
        }
    }
}
