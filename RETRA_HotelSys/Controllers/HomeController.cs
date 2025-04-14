using System.Diagnostics;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RETRA_HotelSys.Data;
using RETRA_HotelSys.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;


namespace RETRA_HotelSys.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<HotelGuests> _userManager;

        public HomeController(
            ILogger<HomeController> logger,
            ApplicationDbContext context,
            UserManager<HotelGuests> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CheckAvailability(DateTime checkInDate, DateTime checkOutDate,
            int numberOfAdults, int numberOfChildren = 0)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid input data";
                return RedirectToAction("Index");
            }

            if (checkInDate >= checkOutDate)
            {
                TempData["ErrorMessage"] = "Check-out date must be after check-in date";
                return RedirectToAction("Index");
            }

            try
            {
                var bookedRoomIds = _context.GuestReservations
                    .Where(r => !(checkOutDate <= r.CheckInDate || checkInDate >= r.CheckOutDate))
                    .Select(r => r.RoomId)
                    .Distinct()
                    .ToList();

                var availableRooms = _context.HotelRooms
                    .Where(r => !bookedRoomIds.Contains(r.RoomId) &&
                                r.MaxOccupancy >= (numberOfAdults + numberOfChildren))
                    .ToList();

                if (availableRooms.Any())
                {
                    TempData["CheckInDate"] = checkInDate.ToString("yyyy-MM-dd");
                    TempData["CheckOutDate"] = checkOutDate.ToString("yyyy-MM-dd");
                    TempData["NumberOfAdults"] = numberOfAdults;
                    TempData["NumberOfChildren"] = numberOfChildren;
                    return RedirectToAction("AvailableRooms", "Booking");
                }

                TempData["ErrorMessage"] = "No available rooms for the selected dates. Please try different dates.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking room availability");
                TempData["ErrorMessage"] = "An error occurred while checking availability";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["ShowLogoutNotification"] = "Logged out successfully!";
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Policy = "GuestOnly")]
        public async Task<IActionResult> GuestHomepage()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                return View(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading guest homepage");
                return RedirectToAction("Error");
            }
        }
    }
}