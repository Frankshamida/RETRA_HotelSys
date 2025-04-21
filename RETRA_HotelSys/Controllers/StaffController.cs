using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RETRA_HotelSys.Data;
using RETRA_HotelSys.Models;
using RETRA_HotelSys.Utilities;

namespace RETRA_HotelSys.Controllers
{
    public class StaffController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StaffController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(StaffLoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var staffMember = await _context.StaffMembers
                    .Include(s => s.StaffRole)
                    .FirstOrDefaultAsync(s => s.Username == model.Username && s.IsActive);

                if (staffMember != null)
                {
                    // Use the PasswordHasher to verify
                    if (PasswordHasher.VerifyPassword(model.Password, staffMember.PasswordHash))
                    {
                        // Update last login
                        staffMember.LastLoginDate = DateTime.Now;
                        _context.Update(staffMember);
                        await _context.SaveChangesAsync();

                        // Create claims
                        var claims = new[]
                        {
                    new Claim(ClaimTypes.Name, staffMember.Username),
                    new Claim("FullName", $"{staffMember.FirstName} {staffMember.LastName}"),
                    new Claim("StaffId", staffMember.StaffId.ToString()),
                    new Claim(ClaimTypes.Role, staffMember.StaffRole.RoleName)
                };

                        var identity = new ClaimsIdentity(claims, "StaffCookies");
                        var principal = new ClaimsPrincipal(identity);

                        // Sign in with the "StaffCookies" scheme
                        await HttpContext.SignInAsync(
                            "StaffCookies",
                            principal,
                            new AuthenticationProperties
                            {
                                IsPersistent = model.RememberMe,
                                ExpiresUtc = model.RememberMe ? DateTime.UtcNow.AddDays(30) : null
                            });

                        if (Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Dashboard");
                        }
                    }
                }

                ModelState.AddModelError(string.Empty, "Invalid username or password");
            }

            return View(model);
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = "StaffCookies")]
        public async Task<IActionResult> Dashboard()
        {
            var staffId = int.Parse(User.FindFirst("StaffId").Value);
            var staffMember = await _context.StaffMembers
                .Include(s => s.StaffRole)
                .FirstOrDefaultAsync(s => s.StaffId == staffId);

            ViewBag.RoleName = staffMember?.StaffRole?.RoleName ?? "Staff";
            return View();
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "StaffCookies")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("StaffCookies");
            return RedirectToAction("Login", "Staff");
        }
    }
}