namespace GabriniCosmetics.Areas.Admin.Models.Interface
{
    public interface IFlagService
    {
        Task<List<Flag>> GetAllFlagsAsync();
        Task<Flag> GetFlagByNameAsync(string name);
    }
}
