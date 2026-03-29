using GabriniCosmetics.Areas.Admin.Models;
using GabriniCosmetics.Areas.Admin.Models.CustomerInfo;

namespace GabriniCosmetics.Models.ViewModel
{
    public class OrderVM
    {
        public ShoppingCart ShoppingCart { get; set; }
        public List<Address> Address { get; set; }
        public IEnumerable<Order> Orders { get; set; }
        public List<OrderItems> OrderItems { get; set; } = new List<OrderItems>();
    }
}
