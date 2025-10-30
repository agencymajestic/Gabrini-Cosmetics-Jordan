using GabriniCosmetics.Areas.Admin.Models.DTOs;

namespace GabriniCosmetics.Areas.Admin.Models.Interface
{
    public interface ICategory
    {
        Task<CategoryDTO> CreateCategory(CreateCategoryDTO createCategoryDto);
        Task<CategoryDTO> GetCategoryById(int id);
        Task<List<CategoryDTO>> GetCategories();
        Task<CategoryDTO> UpdateCategory(UpdateCategoryDTO updateCategoryDto);
        Task<bool> DeleteCategory(int id);
        IQueryable<Category> Queryable();
        Task<int> GetCategoryCountAsync();
    }
}
