using GabriniCosmetics.Areas.Admin.Models.DTOs;
using GabriniCosmetics.Areas.Admin.Models.Interface;
using GabriniCosmetics.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GabriniCosmetics.Areas.Admin.Models.Services
{
    public class SubcategoryService : ISubcategory
    {
        private readonly GabriniCosmeticsContext _context;
        private readonly ILogger<SubcategoryService> _logger;

        public SubcategoryService(GabriniCosmeticsContext context, ILogger<SubcategoryService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<SubcategoryDTO>> GetSubcategories()
        {
            try
            {
                return await _context.Subcategories
                    .AsNoTracking()
                    .Select(sc => new SubcategoryDTO
                    {
                        Id = sc.Id,
                        NameEn = sc.NameEn,
                        NameAr = sc.NameAr,
                        CategoryId = sc.CategoryId
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subcategories");
                throw;
            }
        }

        public async Task<SubcategoryDTO> CreateSubcategory(CreateSubcategoryDTO createSubcategoryDto)
        {
            var categoryId = createSubcategoryDto.CategoryId ?? throw new ArgumentNullException("CategoryId");

            try
            {
                var subcategory = new Subcategory
                {
                    NameEn = createSubcategoryDto.NameEn,
                    NameAr = createSubcategoryDto.NameAr,
                    CategoryId = categoryId
                    // Add other properties as needed
                };

                await _context.Subcategories.AddAsync(subcategory);
                await _context.SaveChangesAsync();

                return new SubcategoryDTO { Id = subcategory.Id, NameEn = subcategory.NameEn, NameAr = subcategory.NameAr };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating subcategory");
                throw;
            }
        }

        public async Task<SubcategoryDTO> GetSubcategoryById(int id)
        {
            try
            {
                var subcategory = await _context.Subcategories
                    .AsNoTracking()
                    .FirstOrDefaultAsync(sc => sc.Id == id);

                if (subcategory == null)
                    return null;

                return new SubcategoryDTO { Id = subcategory.Id, NameEn = subcategory.NameEn, NameAr = subcategory.NameAr, CategoryId = subcategory.CategoryId };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting subcategory by id {id}");
                throw;
            }
        }

        public async Task<SubcategoryDTO> UpdateSubcategory(UpdateSubcategoryDTO updateSubcategoryDto)
        {
            var categoryId = updateSubcategoryDto.CategoryId ?? throw new ArgumentNullException("CategoryId");
            try
            {
                var existingSubcategory = await _context.Subcategories.FindAsync(updateSubcategoryDto.Id);
                if (existingSubcategory == null)
                {
                    return null;
                }

                existingSubcategory.NameEn = updateSubcategoryDto.NameEn;
                existingSubcategory.NameAr = updateSubcategoryDto.NameAr;
                existingSubcategory.CategoryId = categoryId;
                // Update other properties as needed

                await _context.SaveChangesAsync();

                return new SubcategoryDTO { Id = existingSubcategory.Id, NameEn = existingSubcategory.NameEn, NameAr = existingSubcategory.NameAr };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating subcategory with id {updateSubcategoryDto.Id}");
                throw;
            }
        }

        public async Task<bool> DeleteSubcategory(int id)
        {
            try
            {
                var subcategory = await _context.Subcategories.FindAsync(id);
                if (subcategory == null)
                    return false;

                _context.Subcategories.Remove(subcategory);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting subcategory with id {id}");
                throw;
            }
        }

        public async Task<int> GetSubCategoryCountAsync()
        {
            return await _context.Subcategories.CountAsync();
        }
    }
}
