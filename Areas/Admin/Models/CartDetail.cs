using GabriniCosmetics.Areas.Admin.Models.Interface;
using System.ComponentModel.DataAnnotations;

namespace GabriniCosmetics.Areas.Admin.Models
{
    public class CartDetail
    {
        public int Id { get; set; }
        [Required]
        public int ShoppingCartId { get; set; }
        [Required]
        //public int ProductId { get; set; }
        public int SubproductId { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public double UnitPrice { get; set; }
        public string Image { get; set; } = string.Empty;
        //public Product Products { get; set; }
        public Subproduct Subproduct { get; set; }
        public ShoppingCart ShoppingCart { get; set; }

    }
}
