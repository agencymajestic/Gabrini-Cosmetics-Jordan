using GabriniCosmetics.Areas.Admin.Models.DTOs;
using GabriniCosmetics.Areas.Admin.Models.Interface;
using GabriniCosmetics.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;

namespace GabriniCosmetics.Areas.Admin.Models.Services
{
    public class UserService : IUserService
    {
        private readonly GabriniCosmeticsContext _context;

        public UserService(GabriniCosmeticsContext context)
        {
            _context = context;
        }


        public async Task<UserDto> GetUserDetails(string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return new UserDto();
            }
            var orders = await _context.Orders.Where(o => o.UserID == userId).ToListAsync();

            return new UserDto()
            {
                Id = userId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender,
                Orders = orders
            };
        }

        public async Task<IEnumerable<UserDto>> GetUsers(string? searchTerm = null)
        {
            var query = _context.Users.Where(u => u.Email != "unknown@GabriniCosmetics.com" );
            //&& !(u.Id.Contains(""))

            if (!searchTerm.IsNullOrEmpty())
            {
                query = query.Where(u => u.FirstName.ToLower().Trim().Contains(searchTerm.ToLower().Trim()))
                .Where(u => u.LastName.ToLower().Trim().Contains(searchTerm.ToLower().Trim()))
                .Where(u => u.City.ToLower().Trim().Contains(searchTerm.ToLower().Trim()))
                .Where(u => u.PhoneNumber.ToLower().Trim().Contains(searchTerm.ToLower().Trim()))
                .Where(u => u.Email.ToLower().Trim().Contains(searchTerm.ToLower().Trim()));
            }
            
            var usersdata = await query.Select(u => new UserDto()
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                DateOfBirth = u.DateOfBirth,
                Gender = u.Gender,
                City = u.City,
                PhoneNumber = u.PhoneNumber
            }).ToListAsync();

            foreach (var item in usersdata)
            {
                var address = await _context.Addresses.Where(x => x.UserId == item.Id).FirstOrDefaultAsync();
                if (address != null)
                {
                    item.PhoneNumber = (address.PhoneNumber == null || address.PhoneNumber == "") ? "N/A" : address.PhoneNumber;
                    item.City = (address.City == null || address.City == "") ? "N/A" : address.City;
                }
                else
                {
                    item.PhoneNumber = "N/A";
                    item.City = "N/A";
                }
            }
            return usersdata;


        }
    }
}
