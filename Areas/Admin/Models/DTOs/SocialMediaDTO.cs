using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GabriniCosmetics.Areas.Admin.Models.DTOs
{
    public class SocialMediaForm
    {
        public int? Id { get; set; }
        public string Url { get; set; }
        public IFormFile Image { get; set; }
        public string? ExistingImagePath { get; set; }
    }
}