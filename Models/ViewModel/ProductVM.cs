using GabriniCosmetics.Areas.Admin.Models;
using GabriniCosmetics.Areas.Admin.Models.DTOs;

namespace GabriniCosmetics.Models.ViewModel
{
    public class ProductVM
    {
        public ProductDTO Products { get; set; }
        public List<ProductDTO> ProductBySubCategory { get; set; }
    }
}
