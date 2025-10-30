using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GabriniCosmetics.Areas.Admin.Models
{
    public class Subcategory
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "The Name in English is required.")]
        public string NameEn { get; set; }

        [Required(ErrorMessage = "The Name in Arabic is required.")]
        public string NameAr { get; set; }

        // Foreign Key relation to Category
        public int CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; }
        public ICollection<Product> Products { get; set; }

        // Add more properties as needed

        // Example: If Subcategory has a list of related products
        // public List<Product> Products { get; set; }
    }
}
