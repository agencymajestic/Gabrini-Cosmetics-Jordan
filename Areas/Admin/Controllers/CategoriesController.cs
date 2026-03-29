using AutoMapper;
using GabriniCosmetics.Areas.Admin.Models;
using GabriniCosmetics.Areas.Admin.Models.DTOs;
using GabriniCosmetics.Areas.Admin.Models.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GabriniCosmetics.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly ICategory _categoryService;
        private readonly IMapper _mapper;

        public CategoriesController(IWebHostEnvironment environment, ICategory categoryService, IConfiguration configuration,IMapper mapper)
        {
            _categoryService = categoryService;
            _mapper = mapper;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            var listOfCategoriessss = await _categoryService.Queryable().ToListAsync();
            var listOfCategories =  _mapper.Map<List<CategoryDTO>> (listOfCategoriessss);
            return View(listOfCategories);
        }

        // GET: Categories/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCategoryDTO createCategoryDto)
        {
            if (ModelState.IsValid)
            {
                await _categoryService.CreateCategory(createCategoryDto);
                return Redirect("/Admin/Categories");
            }
            return View(createCategoryDto);
        }

        // GET: Categories/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var categoryDto = await _categoryService.GetCategoryById(id);
            if (categoryDto == null)
            {
                return NotFound();
            }

            var updateCategoryDto = new UpdateCategoryDTO
            {
                Id = id,
                NameEn = categoryDto.NameEn,
                NameAr = categoryDto.NameAr,
                ExistingImagePath = categoryDto.ImageUpload
            };

            return View(updateCategoryDto);
        }

        // POST: Categories/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateCategoryDTO updateCategoryDto)
        {
            if(updateCategoryDto.ImageUpload == null && updateCategoryDto.ExistingImagePath == null)
            {
                var categoryDto = await _categoryService.GetCategoryById(updateCategoryDto.Id);
                updateCategoryDto.ExistingImagePath = categoryDto.ImageUpload;
            }
            var updatedCategory = await _categoryService.UpdateCategory(updateCategoryDto);
            if (updatedCategory == null)
            {
                return NotFound();
            }
            else
                return Redirect("/Admin/Categories");
        }

        // POST: Categories/Delete/{id}
        [HttpPost]
        public async Task<IActionResult> DeleteProduct([FromBody] int id)
        {
            var success = await _categoryService.DeleteCategory(id);
            if (!success)
            {
                return NotFound();
            }
            return Ok();
        }
    }
}
