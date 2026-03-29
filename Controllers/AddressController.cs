using GabriniCosmetics.Areas.Admin.Models.CustomerInfo;
using GabriniCosmetics.Areas.Admin.Models.Interface;
using GabriniCosmetics.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GabriniCosmetics.Controllers
{
    public class AddressController : Controller
    {
        private readonly IAddressService _addressService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AddressController(IAddressService addressService, UserManager<ApplicationUser> userManager)
        {
            _addressService = addressService;
            _userManager = userManager;
        }

        // GET: Address
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var addresses = await _addressService.GetAddressByUserIdAsync(user.Id);
            return View(addresses);
        }

        // GET: Address/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var address = await _addressService.GetAddressByIdAsync(id.Value);
            if (address == null)
            {
                return NotFound();
            }

            return View(address);
        }

        // GET: Address/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Address/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Email,Country,City,Address1,Address2,PhoneNumber,FaxNumber")] Address address)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                address.UserId = user.Id;
                await _addressService.CreateAddressAsync(address);
                return RedirectToAction(nameof(Index));
            }
            return BadRequest();
        }

        // GET: Address/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var address = await _addressService.GetAddressByIdAsync(id.Value);
            if (address == null)
            {
                return NotFound();
            }
            return View(address);
        }

        // POST: Address/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Email,Country,City,Address1,Address2,PhoneNumber,FaxNumber")] Address address)
        {
            var user = await _userManager.GetUserAsync(User);

            if (id != address.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _addressService.UpdateAddressAsync(address, user.Id);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_addressService.AddressExists(address.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                ViewData["SuccessMessage"] = "Address updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(address);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            try
            {
                await _addressService.DeleteAddressAsync(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }

}
