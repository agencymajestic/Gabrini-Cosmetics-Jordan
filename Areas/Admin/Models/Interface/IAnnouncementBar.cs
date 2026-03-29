namespace GabriniCosmetics.Areas.Admin.Models.Interface
{
    public interface IAnnouncementBar
    {
        Task<List<AnnouncementBar>> GetAllAsync();
        Task<AnnouncementBar> GetByIdAsync(int id);
        Task<AnnouncementBar> CreateAsync(AnnouncementBar announcementBar);
        Task<AnnouncementBar> UpdateAsync(AnnouncementBar announcementBar);
        Task<bool> DeleteAsync(int id);
    }
}
