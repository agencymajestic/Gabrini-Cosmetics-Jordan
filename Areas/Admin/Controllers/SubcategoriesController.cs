using GabriniCosmetics.Areas.Admin.Models.DTOs;
using GabriniCosmetics.Areas.Admin.Models.Interface;
using GabriniCosmetics.Areas.Admin.Models.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace GabriniCosmetics.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SubcategoriesController : Controller
    {
        private readonly ISubcategory _subcategoryService;
        private readonly ICategory _categoryService;

        public SubcategoriesController(ISubcategory subcategoryService, ICategory categoryService)
        {
            _subcategoryService = subcategoryService;
            _categoryService = categoryService;
        }

        // GET: Subcategories
        public async Task<IActionResult> Index()
        {
            var listOfSubcategories = await _subcategoryService.GetSubcategories();
            foreach (var item in listOfSubcategories)
            {
                var category = await _categoryService.GetCategoryById(item.CategoryId);
                item.CategoryNameEn = category.NameEn;
                item.CategoryNameAr = category.NameAr;
            }
            return View(listOfSubcategories);
        }

        // GET: Subcategories/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var listOfCategories = await _categoryService.GetCategories();
            ViewBag.Categories = new SelectList(listOfCategories, "Id", "NameEn"); // Adjust this based on your actual category model

            return View();
        }

        // POST: Subcategories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateSubcategoryDTO createSubcategoryDto)
        {
            if (ModelState.IsValid)
            {
                await _subcategoryService.CreateSubcategory(createSubcategoryDto);
                return Ok();
            }

            // Return errors with keys
            var errors = ModelState.Where(ms => ms.Value.Errors.Count > 0)
                                   .ToDictionary(
                                       kvp => kvp.Key, // Property name (key)
                                       kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList() // List of errors
                                   );

            return BadRequest(new { success = false, errors });

        }

        // GET: Subcategories/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var subcategoryDto = await _subcategoryService.GetSubcategoryById(id);
            var category = await _categoryService.GetCategoryById(subcategoryDto.CategoryId);

            var listOfCategories = await _categoryService.GetCategories();
            ViewBag.Categories = new SelectList(listOfCategories, "Id", "NameEn"); // Adjust this based on your actual category model

            if (subcategoryDto == null)
            {
                return NotFound();
            }

            var updateSubcategoryDto = new UpdateSubcategoryDTO
            {
                Id = id,
                NameEn = subcategoryDto.NameEn,
                NameAr = subcategoryDto.NameAr,
                CategoryId = category.Id
            };

            return View(updateSubcategoryDto);
        }

        // POST: Subcategories/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateSubcategoryDTO updateSubcategoryDto)
        {

            if (ModelState.IsValid)
            {
                var updatedSubcategory = await _subcategoryService.UpdateSubcategory(updateSubcategoryDto);

                if (updatedSubcategory == null)
                {
                    return NotFound();
                }

                return Ok();
            }

            // Return errors with keys
            var errors = ModelState.Where(ms => ms.Value.Errors.Count > 0)
                                   .ToDictionary(
                                       kvp => kvp.Key, // Property name (key)
                                       kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList() // List of errors
                                   );

            return BadRequest(new { success = false, errors });
        }

        // POST: Subcategories/Delete/{id}
        [HttpPost]
        public async Task<IActionResult> DeleteSubcategory([FromBody] int id)
        {
            var success = await _subcategoryService.DeleteSubcategory(id);
            if (!success)
            {
                return NotFound();
            }
            return Ok();
        }
    }
}
