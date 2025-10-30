namespace GabriniCosmetics.Areas.Admin.Models.Interface
{
    public interface IDiscountService
    {
        Task<Discount> GetDiscountByCodeAsync(string code);
        Task<IEnumerable<Discount>> GetAllDiscountsAsync();
        Task<Discount> GetDiscountById(int id);
        Task AddDiscountAsync(Discount discount);
        Task<Discount> UpdateDiscount(Discount updateDiscount);
        Task<bool> Delete(int id);

        Task<(bool IsSuccess, decimal DiscountedPrice)> ApplyDiscountAsync(string code, decimal originalPrice);
    }
}
