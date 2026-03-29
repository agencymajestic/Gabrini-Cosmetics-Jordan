using System.ComponentModel.DataAnnotations;

namespace GabriniCosmetics.Areas.Admin.Models.ViewModel
{
    public class DiscountViewModel
    {
        [Required]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "Code must be 8 characters long.")]
        public string Code { get; set; }

        [Required]
        [Range(0, 100, ErrorMessage = "Percentage must be between 0 and 100.")]
        public decimal Percentage { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Valid From")]
        public DateTime ValidFrom { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Valid To")]
        public DateTime ValidTo { get; set; }

    }
}
