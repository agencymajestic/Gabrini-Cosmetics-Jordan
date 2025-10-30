using System.ComponentModel.DataAnnotations;

namespace GabriniCosmetics.Areas.Admin.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public string NameEn { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public string NameAr { get; set; }

        public string ImageUpload { get; set; }

        // Navigation property for related subcategories
        public ICollection<Subcategory> SubCategories { get; set; } = new List<Subcategory>();

    }
}
