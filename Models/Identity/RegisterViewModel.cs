using System.ComponentModel.DataAnnotations;

namespace GabriniCosmetics.Models.Identity
{
    public class RegisterViewModel
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Day of birth is required.")]
        [Range(1, 31, ErrorMessage = "Invalid day.")]
        public int DateOfBirthDay { get; set; }

        [Required(ErrorMessage = "Month of birth is required.")]
        [Range(1, 12, ErrorMessage = "Invalid month.")]
        public int DateOfBirthMonth { get; set; }

        [Required(ErrorMessage = "Year of birth is required.")]
        [Range(1900, 2100, ErrorMessage = "Invalid year.")]
        public int DateOfBirthYear { get; set; }
    }

}
