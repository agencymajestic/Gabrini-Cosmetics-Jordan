using GabriniCosmetics.Areas.Admin.Models.DTOs;

namespace GabriniCosmetics.Areas.Admin.Models.Interface
{
    public interface IUserService
    {
        public Task<UserDto> GetUserDetails(string userId);
        public Task<IEnumerable<UserDto>> GetUsers(string? serachWord = null);
    }
}