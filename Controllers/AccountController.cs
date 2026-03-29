using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using GabriniCosmetics.Areas.Admin.Models;
using GabriniCosmetics.Areas.Admin.Models.Interface;
using GabriniCosmetics.Data;
using GabriniCosmetics.Models.Identity;
using GabriniCosmetics.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GabriniCosmetics.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMailService _mailService;
        string culture = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, IMailService mailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _mailService = mailService;
        }
        [HttpGet]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                try
                {
                    var user = new ApplicationUser
                    {
                        UserName = model.Email,
                        Email = model.Email,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Gender = model.Gender,
                        DateOfBirth = new DateTime(model.DateOfBirthYear, model.DateOfBirthMonth, model.DateOfBirthDay)
                    };
                    var result = await _userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        try
                        {
                            // Ensure the role exists
                            if (!await _roleManager.RoleExistsAsync("User"))
                            {
                                var roleResult = await _roleManager.CreateAsync(new IdentityRole("User"));
                                if (!roleResult.Succeeded)
                                {
                                    ModelState.AddModelError(string.Empty, "Failed to create user role.");
                                    return View(model);
                                }
                            }

                            // Assign the "User" role to the new user
                            var roleAssignmentResult = await _userManager.AddToRoleAsync(user, "User");
                            if (!roleAssignmentResult.Succeeded)
                            {
                                ModelState.AddModelError(string.Empty, "Failed to assign user role.");
                                return View(model);
                            }

                            await _signInManager.SignInAsync(user, isPersistent: false);
                            return LocalRedirect(returnUrl ?? Url.Action("Index", "Home"));
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError(string.Empty, $"An error occurred while assigning the role: {ex.Message}");
                        }
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"An error occurred while creating the user: {ex.Message}");
                }
            }
            return View(model);
        }
        public IActionResult AccessDenied()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var model = new RegisterViewModel
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Gender = user.Gender,
                DateOfBirthDay = user.DateOfBirth.Day,
                DateOfBirthMonth = user.DateOfBirth.Month,
                DateOfBirthYear = user.DateOfBirth.Year,
            };

            return View(model);
        }
        [Authorize]
        public IActionResult Addresses()
        {
            return View();
        }
        [Authorize]
        public IActionResult AddAddresses()
        {
            return View();
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditProfile(RegisterViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Gender = model.Gender;
            user.DateOfBirth = new DateTime(model.DateOfBirthYear, model.DateOfBirthMonth, model.DateOfBirthDay);

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            return RedirectToAction("Profile");
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, true, false);
                if (result.Succeeded)
                {
                    return LocalRedirect(returnUrl ?? Url.Action("Index", "Home"));
                }

                var errorMsg = culture.StartsWith("en")
                    ? "Login was unsuccessful. Please correct the errors and try again."
                    : "لم يتم تسجيل الدخول بنجاح. يرجى تصحيح الأخطاء وإعادة المحاولة";

                var additionalError = culture.StartsWith("en")
                    ? " The credentials provided are incorrect."
                    : " إسم المستخدم او كلمة المرور غير صحيحين.";

                errorMsg += additionalError;

                ModelState.AddModelError(string.Empty, errorMsg);
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            var passwordCorrect = await _userManager.CheckPasswordAsync(user, model.OldPassword);

            if (!passwordCorrect)
            {
                var errorMessage = culture.StartsWith("en") ? "Old password doesn't match" : "كلمة المرور القديمة غير صحيحة";
                ModelState.AddModelError(string.Empty, errorMessage);
                return View(model);
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            var passChanged = culture.StartsWith("en") ? "Password was changed." : "تم تغيير كلمة المرر.";
            TempData["BarNotificationDiv"] = "<div id=\"bar-notification\" class=\"bar-notification-container\" data-close=\"Close\">" +
                "<div class=\"bar-notification success\" style=\"display: block;\">" +
                "<p class=\"content\">" + passChanged + "</p>" +
                "<span class=\"close\" title=\"Close\"></span>" +
                "</div></div>";

            TempData["Message"] = "Password changed successfully";
            return View();
        }
        public IActionResult ResetPassword()
        {
            if(!Request.Query.TryGetValue("email", out var email) || !Request.Query.TryGetValue("token", out var token))
            {
                return RedirectToAction("Login", "Account");
            }

            TempData["Email"] = email;
            TempData["Token"] = token;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            var Message = culture.StartsWith("en") ? "Password reset failed" : "فشلت إعادة تعيين كلمة المرور";
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Token))
            {
                TempData["resetPasswordText"] = Message;
                TempData["resetPasswordResult"] = "failed";
                return RedirectToAction("Login", "Account");
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null)
            {
                TempData["resetPasswordText"] = Message;
                TempData["resetPasswordResult"] = "failed";
                return RedirectToAction("Login", "Account");
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
            {
                Message = culture.StartsWith("en") ? "Password reset successful" : "تمت إعادة تعيين كلمة المرور بنجاح";
                TempData["resetPasswordText"] = Message;
                TempData["resetPasswordResult"] = "success";
                return RedirectToAction("Login", "Account");
            }

            
            TempData["resetPasswordText"] = Message;
            TempData["resetPasswordResult"] = "failed";
            return RedirectToAction("Login", "Account");
        }

        public IActionResult PasswordRecovery()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PasswordRecovery(string email)
        {
            var Message = culture.StartsWith("en") ? "Password recovery failed" : "فشلت استعادة كلمة المرور";
            if (string.IsNullOrWhiteSpace(email))
            {
                TempData["resetPasswordRequestText"] = Message;
                TempData["resetPasswordRequestResult"] = "failed";
                return RedirectToAction("Login", "Account");
            }
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
            {
                Message = culture.StartsWith("en") ? "Password recovery email sent" : "تم إرسال بريد إلكتروني لاستعادة كلمة المرور";
                TempData["resetPasswordRequestText"] = Message;
                TempData["resetPasswordRequestResult"] = "success";
                return RedirectToAction("Login", "Account");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action("ResetPassword", "Account", new { email = user.Email, token }, Request.Scheme);

            // Send email
            var mailRequest = new MailRequest
            {
                ToEmail = email,
                Subject = "Reset Password",
                Body = $"Please reset your password by <a href='{callbackUrl}'>clicking here</a>."
            };

            try{
                await _mailService.SendEmailAsync(mailRequest);
            }
            catch
            {
                TempData["resetPasswordRequestText"] = Message;
                TempData["resetPasswordRequestResult"] = "failed";
                return RedirectToAction("Login", "Account");
            }

            Message = culture.StartsWith("en") ? "Password recovery email sent" : "تم إرسال بريد إلكتروني لاستعادة كلمة المرور";
            TempData["resetPasswordRequestText"] = Message;
            TempData["resetPasswordRequestResult"] = "success";

            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}
