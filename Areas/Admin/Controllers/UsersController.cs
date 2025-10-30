using GabriniCosmetics.Areas.Admin.Models.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GabriniCosmetics.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] string? searchTerm = null)
        {
            var users = await _userService.GetUsers(searchTerm);
            return View(users);
        }
        [HttpGet]
        public async Task<IActionResult> Details([FromRoute] string id)
        {
            var user = await _userService.GetUserDetails(id);
            return View(user);
        }
    }
}
