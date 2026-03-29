using GabriniCosmetics.Areas.Admin.Models.DTOs;
using GabriniCosmetics.Areas.Admin.Models.Interface;
using GabriniCosmetics.Data;
using GabriniCosmetics.Models.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace GabriniCosmetics.Areas.Admin.Models.Services
{
    public class DiscountService : IDiscountService
    {
        // Assume this is your data context or a repository pattern.
        private readonly List<Discount> _discounts;
        private readonly GabriniCosmeticsContext _context;

        public DiscountService(GabriniCosmeticsContext context)
        {
            // Initialize with some sample discounts (normally you would use a database).
            _discounts = new List<Discount>();
            _context = context;
        }

        public async Task<Discount> GetDiscountByCodeAsync(string code)
        {
            return await _context.Discounts.AsNoTracking().FirstOrDefaultAsync(x => x.Code == code);
        }

        public async Task<IEnumerable<Discount>> GetAllDiscountsAsync()
        {
            return await _context.Discounts.ToListAsync();
        }
        public async Task<Discount> GetDiscountById(int id)
        {
            return await _context.Discounts.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddDiscountAsync(Discount discount)
        {
            _context.Discounts.Add(discount);
            await _context.SaveChangesAsync();
        }

        public async Task<Discount> UpdateDiscount(Discount updateDiscount)
        {
            var existingDiscount = await _context.Discounts.FindAsync(updateDiscount.Id);
            if (existingDiscount == null)
            {
                throw new KeyNotFoundException($"Category with id {updateDiscount.Id} not found.");
            }

            existingDiscount.Code = updateDiscount.Code;
            existingDiscount.Percentage = updateDiscount.Percentage;
            existingDiscount.ValidFrom = updateDiscount.ValidFrom;
            existingDiscount.ValidTo = updateDiscount.ValidTo;

            await _context.SaveChangesAsync();

            return existingDiscount;
        }

        public async Task<bool> Delete(int id)
        {
            var discount = await _context.Discounts
                .FirstOrDefaultAsync(c => c.Id == id);

            if (discount == null)
                return false;

            _context.Discounts.Remove(discount);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<(bool IsSuccess, decimal DiscountedPrice)> ApplyDiscountAsync(string code, decimal originalPrice)
        {
            var discount = await GetDiscountByCodeAsync(code);

            if (discount == null)
            {
                return (false, originalPrice);
            }
            if(!(DateTime.Now >= discount.ValidFrom && DateTime.Now <= discount.ValidTo))
            {
                return (false, originalPrice);
            }

            decimal discountedPrice = originalPrice - (originalPrice * discount.Percentage / 100);
            return (true, discountedPrice);
        }
    }
}
