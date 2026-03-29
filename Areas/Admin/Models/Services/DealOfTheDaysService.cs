using GabriniCosmetics.Areas.Admin.Models.DTOs;
using GabriniCosmetics.Areas.Admin.Models.Interface;
using GabriniCosmetics.Data;
using Microsoft.EntityFrameworkCore;

namespace GabriniCosmetics.Areas.Admin.Models.Services
{
    public class DealOfTheDaysService : IDealOfTheDaysService
    {
        private readonly GabriniCosmeticsContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public DealOfTheDaysService(GabriniCosmeticsContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IEnumerable<DealOfTheDays>> GetAllDealsAsync()
        {
            return await _context.DealOfTheDays.ToListAsync();
        }

        public async Task<DealOfTheDays> GetDealOfTheDayByIdAsync(int id)
        {
            return await _context.DealOfTheDays.FindAsync(id);
        }

        public async Task UpdateDealOfTheDaysAsync(int id, UpdateDealOfTheDaysDTO dto)
        {
            var deal = await GetDealOfTheDayByIdAsync(id);
            if (deal == null) throw new Exception("Deal not found");

            deal.EndTime = dto.EndTime;
            deal.Id = dto.Id;

            if (dto.ImageUpload != null)
            {
                // Handle image upload
                if (!string.IsNullOrEmpty(dto.ExistingImagePath))
                {
                    string existingFilePath = Path.Combine(_webHostEnvironment.WebRootPath, dto.ExistingImagePath);
                    if (File.Exists(existingFilePath))
                    {
                        File.Delete(existingFilePath);
                    }
                }

                string newFileName = $"{Guid.NewGuid()}_{dto.ImageUpload.FileName}";
                string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/", newFileName);
                using (var fileStream = new FileStream(uploadPath, FileMode.Create))
                {
                    await dto.ImageUpload.CopyToAsync(fileStream);
                }

                deal.ImageUpload = Path.Combine("uploads/", newFileName);
            }

            _context.DealOfTheDays.Update(deal);
            await _context.SaveChangesAsync();
        }
    }

}
