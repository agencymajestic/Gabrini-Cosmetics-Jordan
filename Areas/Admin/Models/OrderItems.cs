using GabriniCosmetics.Areas.Admin.Models.Interface;

namespace GabriniCosmetics.Areas.Admin.Models
{
    public class OrderItems
    {
        public int ID { get; set; }
        public int OrderID { get; set; }
        public int SubproductId { get; set; }
        public int Quantity { get; set; }
        public string ImageProduct { get; set; } = string.Empty;
        public double? TotalPrice { get; set; }
        public decimal? TotalDiscount { get; set; }
        public string? DiscountCode { get; set; }

        public Order Order { get; set; }
        public Subproduct Subproduct { get; set; }

    }
}
