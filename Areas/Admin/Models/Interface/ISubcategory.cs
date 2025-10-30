using GabriniCosmetics.Areas.Admin.Models.DTOs;

namespace GabriniCosmetics.Areas.Admin.Models.Interface
{
    public interface ISubcategory
    {
        Task<List<SubcategoryDTO>> GetSubcategories();
        Task<SubcategoryDTO> CreateSubcategory(CreateSubcategoryDTO createSubcategoryDto);
        Task<SubcategoryDTO> GetSubcategoryById(int id);
        Task<SubcategoryDTO> UpdateSubcategory(UpdateSubcategoryDTO updateSubcategoryDto);
        Task<bool> DeleteSubcategory(int id);
        Task<int> GetSubCategoryCountAsync();

    }
}
