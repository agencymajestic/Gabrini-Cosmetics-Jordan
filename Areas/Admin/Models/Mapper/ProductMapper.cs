using AutoMapper;
using GabriniCosmetics.Areas.Admin.Models.DTOs;
using GabriniCosmetics.Areas.Admin.Models;

namespace GabriniCosmetics.Areas.Admin.Models.Mapper
{
    public class ProductMapper : Profile
    {
        public ProductMapper()
        {
            CreateMap<Product, ProductView>()
            .ForMember(p => p.Subproducts, opt => opt.MapFrom(src => src.Subproducts.OrderBy(sp => sp.Order)));
            CreateMap<Subproduct, SubproductView>();
            CreateMap<ProductFlag, FlagView>();
            CreateMap<ProductImage, ImageView>();
        }
    }
}