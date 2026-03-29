using AutoMapper;
using GabriniCosmetics.Areas.Admin.Models.DTOs;

namespace GabriniCosmetics.Areas.Admin.Models.Mapper
{
    public class CategoryMapper : Profile
    {
        public CategoryMapper() 
        {
            CreateMap<Category, CategoryDTO>();
            CreateMap<CategoryDTO, Category>();


            CreateMap<CreateCategoryDTO, Category>();
            CreateMap<Category, CreateCategoryDTO>();




            CreateMap<Category, CategoryDTO>();
            CreateMap<CategoryDTO, Category>();

            CreateMap<Subcategory, SubCategoryDTO>();
            CreateMap<SubCategoryDTO, Subcategory>();

            CreateMap<Category, CategoryView>();
            CreateMap<Subcategory, SubcategoryView>();
        }
    }
}
