using GabriniCosmetics.Areas.Admin.Models.Interface;

namespace GabriniCosmetics.Areas.Admin.Models.DTOs
{
    public class CategoryDTO
    {
        public int Id { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public string ImageUpload { get; set; }
        public List<SubCategoryDTO> SubCategories { get; set; }
    }

    public class CreateCategoryDTO
    {
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public IFormFile ImageUpload { get; set; }
    }

    public class UpdateCategoryDTO
    {
        public int Id { get; set; } // To hold the category ID
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public IFormFile ImageUpload { get; set; }
        public string ExistingImagePath { get; set; } // To hold the existing image path
    }

    public class SubCategoryDTO
    {
        public int Id { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public CategoryDTO Category { get; set; }
    }

}
