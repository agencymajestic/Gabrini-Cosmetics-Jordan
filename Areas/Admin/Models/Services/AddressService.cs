using GabriniCosmetics.Areas.Admin.Models.CustomerInfo;
using GabriniCosmetics.Areas.Admin.Models.Interface;
using GabriniCosmetics.Data;
using Microsoft.EntityFrameworkCore;

namespace GabriniCosmetics.Models.Services
{
    public class AddressService : IAddressService
    {
        private readonly GabriniCosmeticsContext _context;

        public AddressService(GabriniCosmeticsContext context)
        {
            _context = context;
        }

        public async Task<List<Address>> GetAllAddressesAsync()
        {
            return await _context.Addresses.ToListAsync();
        }

        public async Task<Address> GetAddressByIdAsync(int id)
        {
            return await _context.Addresses.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task CreateAddressAsync(Address address)
        {
            _context.Add(address);
            await _context.SaveChangesAsync();
        }

        public async Task<Address> UpdateAddressAsync(Address updatedAddress, string userId)
        {

            // Find the existing address by ID
            var existingAddress = await _context.Addresses.FindAsync(updatedAddress.Id);
            if (existingAddress == null)
            {
                throw new KeyNotFoundException($"Address with id {updatedAddress.Id} not found.");
            }

            // Update the properties
            existingAddress.UserId = updatedAddress.UserId;
            existingAddress.FirstName = updatedAddress.FirstName;
            existingAddress.LastName = updatedAddress.LastName;
            existingAddress.Email = updatedAddress.Email;
            existingAddress.Country = updatedAddress.Country;
            existingAddress.City = updatedAddress.City;
            existingAddress.Address1 = updatedAddress.Address1;
            existingAddress.Address2 = updatedAddress.Address2;
            existingAddress.PhoneNumber = updatedAddress.PhoneNumber;
            existingAddress.FaxNumber = updatedAddress.FaxNumber;
            existingAddress.UserId = userId;

            // Save the changes to the database
             await _context.SaveChangesAsync();

            return existingAddress;
        }


        public async Task DeleteAddressAsync(int id)
        {
            var address = await _context.Addresses.FindAsync(id);
            if (address != null)
            {
                _context.Addresses.Remove(address);
                await _context.SaveChangesAsync();
            }
        }

        public bool AddressExists(int id)
        {
            return _context.Addresses.Any(e => e.Id == id);
        }

        public async Task<List<Address>> GetAddressByUserIdAsync(string userId)
        {
            return await _context.Addresses.Where(e => e.UserId == userId).ToListAsync();
        }
    }
}
