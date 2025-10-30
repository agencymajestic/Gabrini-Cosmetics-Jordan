using GabriniCosmetics.Areas.Admin.Models;

namespace GabriniCosmetics.Models.ViewModel
{
    public class OrderDetailVM
    {
        public Order Order { get; set; }
        public ShoppingCart ShoppingCart { get; set; }
        public IEnumerable<OrderItems> OrderItems { get; set; }
        public IEnumerable<CartDetail> CartDetail { get; set; }
        public int Tax { get; set; }
        public decimal? PriceBeforeDiscount { get; set; }
        public decimal? PriceAfterDiscount { get; set; }
    }
}
