using GabriniCosmetics.Areas.Admin.Models.DTOs;

namespace GabriniCosmetics.Areas.Admin.Models.Interface
{
    public interface IDealOfTheDaysService
    {
        Task<IEnumerable<DealOfTheDays>> GetAllDealsAsync();
        Task<DealOfTheDays> GetDealOfTheDayByIdAsync(int id);
        Task UpdateDealOfTheDaysAsync(int id, UpdateDealOfTheDaysDTO dto);
    }
}
