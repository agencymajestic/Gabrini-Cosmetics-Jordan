using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace GabriniCosmetics.Models.Identity
{
    public class ResetPasswordModel
    {
        [HiddenInput]
        [EmailAddress]
        [Required]
        public string Email { get; set; }
        [HiddenInput]
        [Required]
        public string Token { get; set; }
        
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Compare("Password", ErrorMessage = "Password and Confirm Password must match")]
        public string ConfirmPassword { get; set; }
    }
}