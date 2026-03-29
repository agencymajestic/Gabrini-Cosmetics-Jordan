using GabriniCosmetics.Areas.Admin.Models.DTOs;

namespace GabriniCosmetics.Areas.Admin.Models.Interface
{
    public interface ISliderAdService
    {
        Task<List<SliderAdDTO>> GetAllSliderAds();
        Task<SliderAdDTO> GetSliderAdById(int id);
        Task UpdateSliderAd(UpdateSliderAdDTO updateSliderAdDto);
        Task CreateSliderAd(CreateSliderAdDTO createSliderAdDto);
        Task<bool> DeleteSliderAd(int id);

    }
}
