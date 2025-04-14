using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RETRA_HotelSys.Data;
using RETRA_HotelSys.Models;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;

[Authorize(Roles = "Admin")] // Restrict entire controller to Admin role only
[Route("Admin")] // Set base route for all actions
[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)] // Disable caching globally for this controller
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<HotelGuests> _userManager;
    private readonly IConfiguration _configuration;

    public AdminController(
        ApplicationDbContext context,
        UserManager<HotelGuests> userManager,
        IConfiguration configuration)
    {
        _context = context;
        _userManager = userManager;
        _configuration = configuration;
    }

    [HttpGet]
    [Route("")] // Handles /Admin
    [Route("Dashboard")] // Handles /Admin/Dashboard
    public async Task<IActionResult> Dashboard()
    {
        // Additional verification for extra security
        var isVerified = HttpContext.Session.GetString("AdminAccessVerified");
        if (isVerified != "true")
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("AdminAccess", "Account");
        }

        // Clear any cached versions
        HttpContext.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
        HttpContext.Response.Headers["Pragma"] = "no-cache";
        HttpContext.Response.Headers["Expires"] = "0";

        // Get current admin user
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var model = new AdminDashboardViewModel
        {
            AdminName = $"{user.FirstName} {user.LastName}",
            AdminEmail = user.Email,
            LastLogin = user.LastLoginDate,
            TotalRooms = _context.HotelRooms.Count(),
            AvailableRooms = _context.HotelRooms.Count(r => r.IsAvailable),
            OccupiedRooms = _context.HotelRooms.Count(r => !r.IsAvailable),
            TotalStaff = _context.StaffMembers.Count(),
            ActiveReservations = _context.GuestReservations.Count(r => r.BookingStatus == "Confirmed"),
            RecentReservations = _context.GuestReservations
                .OrderByDescending(r => r.CheckInDate)
                .Take(5)
                .ToList(),
            MaintenanceRequests = _context.RoomMaintenance
                .Where(m => m.Status == "Pending")
                .OrderByDescending(m => m.Priority)
                .Take(5)
                .ToList(),
            RevenueThisMonth = _context.GuestReservations
                .Where(r => r.CheckInDate.Month == DateTime.Now.Month && r.PaymentStatus == "Paid")
                .Sum(r => r.TotalAmount)
        };

        // Update last access time
        user.LastAccessDate = DateTime.Now;
        await _userManager.UpdateAsync(user);

        return View(model);
    }

    [HttpGet]
    [Route("Profile")]
    public async Task<IActionResult> Profile()
    {
        // Additional verification for extra security
        var isVerified = HttpContext.Session.GetString("AdminAccessVerified");
        if (isVerified != "true")
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("AdminAccess", "Account");
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var model = new AdminProfileViewModel
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            LastLoginDate = user.LastLoginDate,
            LastAccessDate = user.LastAccessDate
        };

        return View(model);
    }

    [HttpPost]
    [Route("Profile")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(AdminProfileViewModel model)
    {
        // Additional verification for extra security
        var isVerified = HttpContext.Session.GetString("AdminAccessVerified");
        if (isVerified != "true")
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("AdminAccess", "Account");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        // Update user properties
        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        user.PhoneNumber = model.PhoneNumber;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

        TempData["SuccessMessage"] = "Your profile has been updated successfully.";
        return RedirectToAction("Profile");
    }

    [HttpGet]
    [Route("Logout")]
    public async Task<IActionResult> Logout()
    {
        // Clear the verification flag
        HttpContext.Session.Remove("AdminAccessVerified");

        await _userManager.UpdateSecurityStampAsync(await _userManager.GetUserAsync(User));
        await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
        return RedirectToAction("Login", "Account");
    }
}