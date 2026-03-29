using GabriniCosmetics.Areas.Admin.Models.DTOs;
using GabriniCosmetics.Areas.Admin.Models.Interface;
using GabriniCosmetics.Areas.Admin.Models.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GabriniCosmetics.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SliderBannerController : Controller
    {
        private readonly ISliderBannerService _sliderBannerService;

        public SliderBannerController(ISliderBannerService sliderBannerService)
        {
            _sliderBannerService = sliderBannerService;
        }

        public async Task<IActionResult> Index()
        {
            var sliderBanners = await _sliderBannerService.GetSliderBanners();
            return View(sliderBanners);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateSliderBannerDTO createSliderBannerDto)
        {
            if (ModelState.IsValid)
            {
                await _sliderBannerService.CreateSliderBanner(createSliderBannerDto);
                return Redirect("/Admin/SliderBanner");
            }
            return View(createSliderBannerDto);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var sliderBanner = await _sliderBannerService.GetSliderBannerById(id);
            if (sliderBanner == null)
            {
                return NotFound();
            }

            var model = new UpdateSliderBannerDTO
            {
                Id = sliderBanner.Id,
                ExistingImagePath = sliderBanner.ImageUrl
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateSliderBannerDTO updateSliderBannerDto)
        {
            // Ensure the existing image path is retained if no new image is uploaded
            if (updateSliderBannerDto.ImageUpload == null && updateSliderBannerDto.ExistingImagePath == null)
            {
                var sliderBannerDto = await _sliderBannerService.GetSliderBannerById(updateSliderBannerDto.Id);
                updateSliderBannerDto.ExistingImagePath = sliderBannerDto.ImageUrl;
            }
            await _sliderBannerService.UpdateSliderBanner(updateSliderBannerDto);
            return Redirect("/Admin/SliderBanner");
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] int id)
        {
            await _sliderBannerService.DeleteSliderBanner(id);
            return Redirect("/Admin/SliderBanner");
        }
    }
}

