using GabriniCosmetics.Areas.Admin.Models.CustomerInfo;
using GabriniCosmetics.Areas.Admin.Models;

namespace GabriniCosmetics.Models.ViewModel
{
    public class CheckoutVM
    {
        public List<Address> Addresses { get; set; }
        public Order Order { get; set; }
        public ShoppingCart ShoppingCart { get; set; }
        public double? TotalPrice { get; set; }
        public string? DiscountCode { get; set; }
    }

    public class StateChanged
    {
        public int StateId { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
