using GabriniCosmetics.Areas.Admin.Models.Interface;
using System.ComponentModel.DataAnnotations;

namespace GabriniCosmetics.Areas.Admin.Models
{
    public class WishlistDetail
    {
        public int Id { get; set; }
        [Required]
        public int WishlistId { get; set; }
        [Required]
        public int SubproductId { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public double UnitPrice { get; set; }
        public string Image { get; set; } = string.Empty;
        public Subproduct Subproduct { get; set; }
        public Wishlist Wishlist { get; set; }

    }
}