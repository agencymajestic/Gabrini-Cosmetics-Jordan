using GabriniCosmetics.Areas.Admin.Models.CustomerInfo;

namespace GabriniCosmetics.Areas.Admin.Models.Interface
{
    public interface IAddressService
    {
        Task<List<Address>> GetAllAddressesAsync();
        Task<Address> GetAddressByIdAsync(int id);
        Task<List<Address>> GetAddressByUserIdAsync(string id);
        Task CreateAddressAsync(Address address);
        Task<Address> UpdateAddressAsync(Address updatedAddress, string userId);
        Task DeleteAddressAsync(int id);
        bool AddressExists(int id);

    }
}
