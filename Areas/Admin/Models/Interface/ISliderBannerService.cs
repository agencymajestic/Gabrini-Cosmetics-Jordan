using GabriniCosmetics.Areas.Admin.Models.DTOs;

namespace GabriniCosmetics.Areas.Admin.Models.Interface
{
    public interface ISliderBannerService
    {
        Task<List<SliderBannerDTO>> GetSliderBanners();
        Task<SliderBannerDTO> CreateSliderBanner(CreateSliderBannerDTO createSliderBannerDto);
        Task<bool> DeleteSliderBanner(int id);
        Task<SliderBannerDTO> GetSliderBannerById(int id);
        Task UpdateSliderBanner(UpdateSliderBannerDTO updateSliderBannerDto);


    }
}
