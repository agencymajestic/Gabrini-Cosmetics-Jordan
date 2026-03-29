using GabriniCosmetics.Areas.Admin.Models.Interface;
using GabriniCosmetics.Data;
using Microsoft.EntityFrameworkCore;

namespace GabriniCosmetics.Areas.Admin.Models.Services
{
    public class ContactUsServices : IContactUs
    {
        private readonly GabriniCosmeticsContext _context;
        public ContactUsServices(GabriniCosmeticsContext context)
        {
            _context = context;
        }

        public async Task<ContactUs> Create(ContactUs contactUs) // Creates a Brands data by saving a Brands object into the connected database
        {
            _context.Entry(contactUs).State = EntityState.Added;
            await _context.SaveChangesAsync();

            return contactUs;
        }
        public async Task<List<ContactUs>> GetFeedback() // Gets all of the Brands data from the connencted database
        {
            return await _context.ContactUs.ToListAsync();
        }

        public async Task<bool> Delete(int id) // Deletes a ContactUs data from the connected database
        {
            var contactUs = await _context.ContactUs.FindAsync(id);
            if (contactUs == null)
            {
                return false; // Entity not found
            }

            _context.ContactUs.Remove(contactUs);
            await _context.SaveChangesAsync();
            return true; // Deletion successful
        }
    }
}
