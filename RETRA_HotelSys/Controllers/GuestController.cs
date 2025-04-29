using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RETRA_HotelSys.Data;
using RETRA_HotelSys.Models;
using RETRA_HotelSys.Models.Guest;
using System.Collections.Concurrent;
using System.Security.Claims;

public class GuestController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<HotelGuests> _userManager;
    private readonly SignInManager<HotelGuests> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private static readonly ConcurrentDictionary<string, (string Otp, DateTime Expiry)> _otpStorage = new();
    private const int OtpExpiryMinutes = 5;

    public GuestController(
        ApplicationDbContext context,
        UserManager<HotelGuests> userManager,
        SignInManager<HotelGuests> signInManager,
        RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(GuestRegistrationViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new HotelGuests
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                DateOfBirth = model.DateOfBirth,
                Address = model.Address,
                City = model.City,
                StateProvince = model.StateProvince,
                Country = model.Country,
                PostalCode = model.PostalCode,
                IDType = model.IDType,
                IDNumber = model.IDNumber,
                RegistrationDate = DateTime.Now,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Assign Guest role
                await _userManager.AddToRoleAsync(user, "Guest");

                // Sign out any existing user
                await _signInManager.SignOutAsync();

                // Redirect to login with success message
                TempData["SuccessMessage"] = "Registration successful! Please log in.";
                return RedirectToAction("Login", "Guest");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return View(model);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    user.LastLoginDate = DateTime.Now;
                    await _userManager.UpdateAsync(user);
                }

                return RedirectToLocal(returnUrl);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }
        }

        return View(model);
    }

    [HttpGet]
    public IActionResult GuestHomepage()
    {
        var model = new RoomSearchViewModel();
        return View(model);
    }


    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword)
    {
        if (string.IsNullOrEmpty(currentPassword) || string.IsNullOrEmpty(newPassword))
        {
            TempData["ErrorMessage"] = "Please fill in all fields.";
            return RedirectToAction("GuestHomepage");
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        // Change the password
        var changePasswordResult = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        if (!changePasswordResult.Succeeded)
        {
            foreach (var error in changePasswordResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            TempData["ErrorMessage"] = "Failed to change password. Please check your current password and try again.";
            return RedirectToAction("GuestHomepage");
        }

        // Refresh the sign-in cookie
        await _signInManager.RefreshSignInAsync(user);

        TempData["SuccessMessage"] = "Your password has been changed successfully.";
        return RedirectToAction("GuestHomepage");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    private IActionResult RedirectToLocal(string returnUrl)
    {
        if (Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }
        else
        {
            return RedirectToAction(nameof(GuestHomepage));
        }
    }

    public IActionResult OurRooms()
    {
        // Replace this with your actual query to get room categories
        var roomCategories = _context.RoomCategories
            .Include(rc => rc.RoomTypeFeatures)
            .ThenInclude(rtf => rtf.RoomFeature)
            .ToList();

        return View(roomCategories);
    }

    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                // Generate OTP (6-digit code)
                var otp = new Random().Next(100000, 999999).ToString();
                var expiry = DateTime.Now.AddMinutes(OtpExpiryMinutes);

                // Store OTP (in production, use a proper storage like database or cache)
                _otpStorage[model.Email] = (otp, expiry);

                // In a real app, you would send this OTP via email/SMS
                // For demo, we'll just show it in the view
                TempData["DemoOtp"] = otp;

                return RedirectToAction("VerifyOtp", new { email = model.Email });
            }

            // Always return success to prevent email enumeration attacks
            return RedirectToAction("VerifyOtp", new { email = model.Email });
        }

        return View(model);
    }

    [HttpGet]
    public IActionResult VerifyOtp(string email)
    {
        var model = new VerifyOtpViewModel { Email = email };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult VerifyOtp(VerifyOtpViewModel model)
    {
        if (ModelState.IsValid)
        {
            if (_otpStorage.TryGetValue(model.Email, out var otpData))
            {
                if (otpData.Otp == model.OtpCode && otpData.Expiry > DateTime.Now)
                {
                    return RedirectToAction("ResetPassword", new
                    {
                        email = model.Email,
                        otpCode = model.OtpCode
                    });
                }

                ModelState.AddModelError(string.Empty, "Invalid or expired OTP code.");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "OTP code not found. Please request a new one.");
            }
        }

        return View(model);
    }

    [HttpGet]
    public IActionResult ResetPassword(string email, string otpCode)
    {
        var model = new ResetPasswordViewModel
        {
            Email = email,
            OtpCode = otpCode
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Verify OTP again
            if (_otpStorage.TryGetValue(model.Email, out var otpData) &&
                otpData.Otp == model.OtpCode &&
                otpData.Expiry > DateTime.Now)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    // Reset password
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

                    if (result.Succeeded)
                    {
                        // Remove used OTP
                        _otpStorage.TryRemove(model.Email, out _);

                        TempData["SuccessMessage"] = "Your password has been reset successfully. Please login with your new password.";
                        return RedirectToAction("Login");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid or expired OTP code.");
            }
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResendOtp(string email)
    {
        if (!string.IsNullOrEmpty(email))
        {
            // Generate new OTP
            var otp = new Random().Next(100000, 999999).ToString();
            var expiry = DateTime.Now.AddMinutes(OtpExpiryMinutes);

            // Update storage
            _otpStorage[email] = (otp, expiry);

            // For demo - in production, send via email/SMS
            TempData["DemoOtp"] = otp;
            TempData["ResendSuccess"] = "New OTP code has been sent";
        }

        return RedirectToAction("VerifyOtp", new { email });
    }

}

