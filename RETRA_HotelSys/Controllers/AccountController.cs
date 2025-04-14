// Controllers/AccountController.cs
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RETRA_HotelSys.Data;
using RETRA_HotelSys.Models;
using System.Threading.Tasks;

public class AccountController : Controller
{
    private readonly SignInManager<HotelGuests> _signInManager;
    private readonly UserManager<HotelGuests> _userManager;
    private readonly IConfiguration _configuration;

    public AccountController(
        SignInManager<HotelGuests> signInManager,
        UserManager<HotelGuests> userManager,
        IConfiguration configuration)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _configuration = configuration;
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("Admin/Access")]
    public IActionResult AdminAccess(string returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("Admin/Access")]
    [ValidateAntiForgeryToken]
    public IActionResult AdminAccess(AdminAccessCodeViewModel model, string returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var validCode = _configuration["AdminAccessSettings:AccessCode"];
        if (model.AccessCode != validCode)
        {
            ModelState.AddModelError(string.Empty, "Invalid access code");
            return View(model);
        }

        // Store verification in session
        HttpContext.Session.SetString("AdminAccessVerified", "true");
        HttpContext.Session.SetString("AdminAccessTime", DateTime.Now.ToString());

        // Always redirect to Login page after successful access code verification
        return RedirectToAction("Login", new { returnUrl = returnUrl });
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
            // Check if access code verification is still valid
            var isVerified = HttpContext.Session.GetString("AdminAccessVerified");
            var accessTime = HttpContext.Session.GetString("AdminAccessTime");

            if (isVerified != "true" ||
                !DateTime.TryParse(accessTime, out var verificationTime) ||
                verificationTime.AddMinutes(_configuration.GetValue<int>("AdminAccessSettings:CodeExpiryMinutes")) < DateTime.Now)
            {
                return RedirectToAction("AdminAccess");
            }

            var result = await _signInManager.PasswordSignInAsync(
                model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null && await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    user.LastLoginDate = DateTime.Now;
                    await _userManager.UpdateAsync(user);

                    // Redirect to dashboard only if coming from admin access flow
                    if (!string.IsNullOrEmpty(returnUrl) && returnUrl.Contains("Admin/Dashboard"))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Dashboard", "Admin");
                }

                return RedirectToLocal(returnUrl);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction(nameof(Login));
    }

    private IActionResult RedirectToLocal(string returnUrl)
    {
        if (Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }
        else
        {
            return RedirectToAction(nameof(AdminController.Dashboard), "Admin");
        }
    }
}