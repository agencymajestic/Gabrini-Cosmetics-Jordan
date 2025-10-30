using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace GabriniCosmetics.Areas.Admin.Models.DTOs
{
    public class SubcategoryDTO
    {
        public int Id { get; set; }

        public string NameEn { get; set; }

        public string NameAr { get; set; }
        public CategoryDTO Category { get; set; }


        //Subcategory has a relation with Category
        public int CategoryId { get; set; }
        public string CategoryNameEn { get; set; }
        public string CategoryNameAr { get; set; }

        // Example: If there are related products to display
        //public List<ProductDTO> Products { get; set; }
    }
    public class CreateSubcategoryDTO
    {
        [Required(ErrorMessage = "Name in English is required")]
        public string NameEn { get; set; }

        [Required(ErrorMessage = "Name in Arabic is required")]
        public string NameAr { get; set; }
        [Required(ErrorMessage = "Please select a category")]
        public int? CategoryId { get; set; }
    }
    public class UpdateSubcategoryDTO
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name in English is required")]
        public string NameEn { get; set; }

        [Required(ErrorMessage = "Name in Arabic is required")]
        public string NameAr { get; set; }
        [Required(ErrorMessage = "Please select a category")]
        public int? CategoryId { get; set; }


    }
}
