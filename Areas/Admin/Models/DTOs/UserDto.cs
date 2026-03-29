using GabriniCosmetics.Areas.Admin.Models.CustomerInfo;
using GabriniCosmetics.Models.ViewModel;

namespace GabriniCosmetics.Areas.Admin.Models.DTOs
{
    public class UserDto
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string City { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public IEnumerable<Order> Orders { get; set; } = Enumerable.Empty<Order>();
        public IEnumerable<Address> Address { get; set; } = Enumerable.Empty<Address>();
    }
}
