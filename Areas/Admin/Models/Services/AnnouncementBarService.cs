using GabriniCosmetics.Areas.Admin.Models.Interface;
using GabriniCosmetics.Data;
using Microsoft.EntityFrameworkCore;

namespace GabriniCosmetics.Areas.Admin.Models.Services
{
    public class AnnouncementBarService : IAnnouncementBar
    {
        private readonly GabriniCosmeticsContext _context;

        public AnnouncementBarService(GabriniCosmeticsContext context)
        {
            _context = context;
        }

        public async Task<List<AnnouncementBar>> GetAllAsync()
        {
            return await _context.AnnouncementBars.ToListAsync();
        }

        public async Task<AnnouncementBar> GetByIdAsync(int id)
        {
            return await _context.AnnouncementBars.FindAsync(id);
        }

        public async Task<AnnouncementBar> CreateAsync(AnnouncementBar announcementBar)
        {
            _context.AnnouncementBars.Add(announcementBar);
            await _context.SaveChangesAsync();
            return announcementBar;
        }

        public async Task<AnnouncementBar> UpdateAsync(AnnouncementBar announcementBar)
        {
            _context.AnnouncementBars.Update(announcementBar);
            await _context.SaveChangesAsync();
            return announcementBar;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var announcementBar = await _context.AnnouncementBars.FindAsync(id);
            if (announcementBar == null)
            {
                return false;
            }

            _context.AnnouncementBars.Remove(announcementBar);
            await _context.SaveChangesAsync();
            return true;
        }
    }

}
