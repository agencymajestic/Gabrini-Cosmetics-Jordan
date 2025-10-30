using GabriniCosmetics.Areas.Admin.Models;
using GabriniCosmetics.Areas.Admin.Models.DTOs;

namespace GabriniCosmetics.Models.ViewModel
{
    public class HomeVM
    {
        public List<CategoryDTO> Categories { get; set; }
        public List<ProductView> Products { get; set; }
        public List<ProductView> ProductsSale { get; set; }
        public List<ProductView> ProductsNew { get; set; }
        public List<ProductView> ProductsFeature { get; set; }
        public List<SliderBannerDTO> SlidersBanner { get; set; }
        public List<SliderAdDTO> SlidersAd { get; set; }
        public List<DealOfTheDays> DealOfTheDays { get; set; }
        public List<ProductView> ListOfProductsDealOfTheDays { get; set; }
    }
}
