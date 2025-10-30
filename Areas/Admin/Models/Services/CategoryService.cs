using GabriniCosmetics.Areas.Admin.Models.DTOs;
using GabriniCosmetics.Areas.Admin.Models.Interface;
using GabriniCosmetics.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace GabriniCosmetics.Areas.Admin.Models.Services
{
    public class CategoryService : ICategory
    {
        private readonly GabriniCosmeticsContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(GabriniCosmeticsContext context, IWebHostEnvironment environment, ILogger<CategoryService> logger)
        {
            _context = context;
            _environment = environment;
            _logger = logger;
        }

        public async Task<List<CategoryDTO>> GetCategories()
        {
            try
            {
                return await _context.Categories
                    .AsNoTracking()
                    .Select(c => new CategoryDTO
                    {
                        Id = c.Id,
                        NameEn = c.NameEn,
                        NameAr = c.NameAr,
                        ImageUpload = c.ImageUpload,
                        SubCategories = c.SubCategories.Select(sc => new SubCategoryDTO
                        {
                            Id = sc.Id,
                            NameEn = sc.NameEn,
                            NameAr = sc.NameAr
                        }).ToList()
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting categories");
                throw;
            }
        }

        public async Task<CategoryDTO> CreateCategory(CreateCategoryDTO createCategoryDto)
        {
            try
            {
                var category = new Category
                {
                    NameEn = createCategoryDto.NameEn,
                    NameAr = createCategoryDto.NameAr
                };

                if (createCategoryDto.ImageUpload != null)
                {
                    var fileName = await SaveImageAsync(createCategoryDto.ImageUpload);
                    category.ImageUpload = fileName;
                }

                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                return new CategoryDTO
                {
                    Id = category.Id,
                    NameEn = category.NameEn,
                    NameAr = category.NameAr,
                    ImageUpload = category.ImageUpload
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category");
                throw;
            }
        }

        public async Task<CategoryDTO> GetCategoryById(int id)
        {
            try
            {
                var category = await _context.Categories
                    .AsNoTracking()
                    .Include(c => c.SubCategories)
                    .Select(c => new CategoryDTO
                    {
                        Id = c.Id,
                        NameEn = c.NameEn,
                        NameAr = c.NameAr,
                        ImageUpload = c.ImageUpload,
                        SubCategories = c.SubCategories.Select(sc => new SubCategoryDTO
                        {
                            Id = sc.Id,
                            NameEn = sc.NameEn,
                            NameAr = sc.NameAr
                        }).ToList()
                    })
                    .FirstOrDefaultAsync(c => c.Id == id);

                return category ?? throw new KeyNotFoundException($"Category with id {id} not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting category by id {id}");
                throw;
            }
        }

        public async Task<CategoryDTO> UpdateCategory(UpdateCategoryDTO updateCategoryDto)
        {
            try
            {
                var existingCategory = await _context.Categories.FindAsync(updateCategoryDto.Id);
                if (existingCategory == null)
                {
                    throw new KeyNotFoundException($"Category with id {updateCategoryDto.Id} not found.");
                }

                existingCategory.NameEn = updateCategoryDto.NameEn;
                existingCategory.NameAr = updateCategoryDto.NameAr;

                if (updateCategoryDto.ImageUpload != null)
                {
                    var fileName = await SaveImageAsync(updateCategoryDto.ImageUpload);
                    existingCategory.ImageUpload = fileName;
                }

                await _context.SaveChangesAsync();

                return new CategoryDTO
                {
                    Id = existingCategory.Id,
                    NameEn = existingCategory.NameEn,
                    NameAr = existingCategory.NameAr,
                    ImageUpload = existingCategory.ImageUpload
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating category with id {updateCategoryDto.Id}");
                throw;
            }
        }

        public async Task<bool> DeleteCategory(int id)
        {
            try
            {
                var category = await _context.Categories
                    .Include(c => c.SubCategories)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (category == null)
                    return false;

                // Remove all subcategories associated with this category
                _context.Subcategories.RemoveRange(category.SubCategories);

                // Remove the category itself
                _context.Categories.Remove(category);

                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting category with id {id}");
                throw;
            }
        }

        private async Task<string> SaveImageAsync(IFormFile imageUpload)
        {
            if (imageUpload == null || imageUpload.Length == 0)
            {
                throw new ArgumentException("Invalid image upload");
            }

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageUpload.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageUpload.CopyToAsync(fileStream);
            }

            return fileName;
        }

        public IQueryable<Category> Queryable()
        {
            return _context.Set<Category>().AsQueryable();
        }

        public async Task<int> GetCategoryCountAsync()
        {
            return await _context.Categories.CountAsync();
        }
    }
}