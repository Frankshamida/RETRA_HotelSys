using System.Diagnostics;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RETRA_HotelSys.Data;
using RETRA_HotelSys.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using RETRA_HotelSys.Models.Guest;
using System.Text.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace RETRA_HotelSys.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<HotelGuests> _userManager;
        private readonly SignInManager<HotelGuests> _signInManager;

        public HomeController(
            ILogger<HomeController> logger,
            ApplicationDbContext context,
            UserManager<HotelGuests> userManager,
            SignInManager<HotelGuests> signInManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
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

        public async Task<IActionResult> CheckAvailability(
            DateTime checkInDate,
            DateTime checkOutDate,
            int numberOfAdults,
            int numberOfChildren)
        {
            // Calculate total guests
            int totalGuests = numberOfAdults + numberOfChildren;

            // Validate dates
            if (checkInDate >= checkOutDate)
            {
                ModelState.AddModelError("", "Check-out date must be after check-in date.");
                return View("Index");
            }

            if (checkInDate < DateTime.Today)
            {
                ModelState.AddModelError("", "Check-in date cannot be in the past.");
                return View("Index");
            }

            // Get rooms that can accommodate the guests
            var availableCategories = await _context.RoomCategories
                .Include(rc => rc.HotelRooms)
                .Where(rc => rc.MaxCapacity >= totalGuests)
                .ToListAsync();

            // If no exact matches, find closest higher capacity rooms
            if (!availableCategories.Any())
            {
                availableCategories = await _context.RoomCategories
                    .Include(rc => rc.HotelRooms)
                    .Where(rc => rc.MaxCapacity > totalGuests)
                    .OrderBy(rc => rc.MaxCapacity)
                    .Take(3)
                    .ToListAsync();

                if (availableCategories.Any())
                {
                    ViewBag.ShowCapacityWarning = true;
                    ViewBag.TotalGuests = totalGuests;
                    ViewBag.CapacityMessage = $"We couldn't find rooms that exactly match your group size of {totalGuests}. Below are the closest available options.";
                }
                else
                {
                    ViewBag.NoRoomsMessage = $"We're sorry, but we don't have any rooms available that can accommodate your group of {totalGuests} guests for the selected dates. " +
                                            $"Our maximum room capacity is {await _context.RoomCategories.MaxAsync(rc => rc.MaxCapacity)} guests. " +
                                            "Please try reducing your group size or contact our reservations team for assistance.";

                    ViewBag.CheckInDate = checkInDate;
                    ViewBag.CheckOutDate = checkOutDate;
                    ViewBag.NumberOfAdults = numberOfAdults;
                    ViewBag.NumberOfChildren = numberOfChildren;
                    ViewBag.TotalGuests = totalGuests;

                    return View(new List<RoomAvailabilityViewModel>());
                }
            }

            // Map to ViewModel
            var model = availableCategories.Select(rc => new RoomAvailabilityViewModel
            {
                RoomTypeId = rc.RoomTypeId,
                TypeName = rc.TypeName,
                Description = rc.Description,
                BasePrice = rc.BasePrice,
                MaxCapacity = rc.MaxCapacity,
                BedConfiguration = rc.BedConfiguration,
                SizeInSqFt = rc.SizeInSqFt,
                ImagePath = rc.MainImagePath
            }).ToList();

            ViewBag.CheckInDate = checkInDate;
            ViewBag.CheckOutDate = checkOutDate;
            ViewBag.NumberOfAdults = numberOfAdults;
            ViewBag.NumberOfChildren = numberOfChildren;
            ViewBag.TotalGuests = totalGuests;

            return View(model);
        }

        [HttpPost]
        public IActionResult BookRoom(int roomTypeId, DateTime checkInDate, DateTime checkOutDate,
                            int numberOfAdults, int numberOfChildren)
        {
            var bookingDetails = new BookingDetails
            {
                CheckInDate = checkInDate,
                CheckOutDate = checkOutDate,
                NumberOfAdults = numberOfAdults,
                NumberOfChildren = numberOfChildren
            };

            TempData["BookingDetails"] = JsonSerializer.Serialize(bookingDetails);
            return RedirectToAction("BookRoom", new { roomTypeId = roomTypeId });
        }

        [HttpGet]
        public IActionResult BookRoom(int roomTypeId)
        {
            var bookingDetailsJson = TempData["BookingDetails"] as string;
            if (string.IsNullOrEmpty(bookingDetailsJson))
            {
                return RedirectToAction("Index", "Home");
            }

            var bookingDetails = JsonSerializer.Deserialize<BookingDetails>(bookingDetailsJson);
            var roomType = _context.RoomCategories
                .Include(rt => rt.HotelRooms)
                .FirstOrDefault(rt => rt.RoomTypeId == roomTypeId);

            if (roomType == null)
            {
                return NotFound();
            }

            var bookedRoomIds = _context.GuestReservations
                .Where(r => r.CheckInDate < bookingDetails.CheckOutDate &&
                           r.CheckOutDate > bookingDetails.CheckInDate &&
                           r.BookingStatus != "Cancelled")
                .Select(r => r.RoomId)
                .ToList();

            var availableRoom = roomType.HotelRooms
                .FirstOrDefault(r => !bookedRoomIds.Contains(r.RoomId));

            if (availableRoom == null)
            {
                return View("RoomUnavailable");
            }

            int nights = (int)(bookingDetails.CheckOutDate - bookingDetails.CheckInDate).TotalDays;
            decimal totalPrice = roomType.BasePrice * nights;

            var model = new BookingViewModel
            {
                RoomTypeId = roomTypeId,
                RoomId = availableRoom.RoomId,
                RoomNumber = availableRoom.RoomNumber,
                RoomTypeName = roomType.TypeName,
                CheckInDate = bookingDetails.CheckInDate,
                CheckOutDate = bookingDetails.CheckOutDate,
                NumberOfAdults = bookingDetails.NumberOfAdults,
                NumberOfChildren = bookingDetails.NumberOfChildren,
                BasePrice = roomType.BasePrice,
                TotalNights = nights,
                TotalPrice = totalPrice,
                BedConfiguration = roomType.BedConfiguration,
                RoomImage = roomType.MainImagePath
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> GuestInformation(BookingViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Repopulate necessary data if validation fails
                var roomType = _context.RoomCategories.Find(model.RoomTypeId);
                if (roomType != null)
                {
                    model.RoomTypeName = roomType.TypeName;
                    model.BedConfiguration = roomType.BedConfiguration;
                    model.RoomImage = roomType.MainImagePath;
                }
                return View("BookRoom", model);
            }

            // Create temporary reservation
            var tempReservation = new TempReservation
            {
                RoomTypeId = model.RoomTypeId,
                RoomId = model.RoomId,
                RoomNumber = model.RoomNumber,
                CheckInDate = model.CheckInDate,
                CheckOutDate = model.CheckOutDate,
                NumberOfAdults = model.NumberOfAdults,
                NumberOfChildren = model.NumberOfChildren,
                TotalPrice = model.TotalPrice,
                SpecialRequests = model.SpecialRequests,
                GuestEmail = model.GuestEmail,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                Address = model.Address,
                CreateAccount = model.CreateAccount
            };

            // Store in TempData with a longer expiration
            TempData["TempReservation"] = JsonSerializer.Serialize(tempReservation);
            TempData.Keep("TempReservation"); // Ensure it's preserved

            return RedirectToAction("Payment");
        }

        [HttpGet]
        public IActionResult Payment()
        {
            var tempReservationJson = TempData["TempReservation"] as string;
            if (string.IsNullOrEmpty(tempReservationJson))
            {
                // Try to get it from TempData again (it might have been preserved)
                tempReservationJson = TempData.Peek("TempReservation") as string;

                if (string.IsNullOrEmpty(tempReservationJson))
                {
                    return RedirectToAction("Index");
                }
            }

            var tempReservation = JsonSerializer.Deserialize<TempReservation>(tempReservationJson);

            // Preserve the data for the POST action
            TempData["TempReservation"] = tempReservationJson;
            TempData.Keep("TempReservation");

            var roomType = _context.RoomCategories.Find(tempReservation.RoomTypeId);

            var model = new PaymentViewModel
            {
                AmountDue = tempReservation.TotalPrice,
                RoomType = roomType?.TypeName,
                RoomNumber = tempReservation.RoomNumber,
                CheckInDate = tempReservation.CheckInDate,
                CheckOutDate = tempReservation.CheckOutDate,
                GuestName = $"{tempReservation.FirstName} {tempReservation.LastName}",
                ReservationId = 0 // Will be set after actual reservation is created
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessPayment(PaymentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Payment", model);
            }

            var tempReservationJson = TempData["TempReservation"] as string;
            if (string.IsNullOrEmpty(tempReservationJson))
            {
                return RedirectToAction("Index");
            }

            var tempReservation = JsonSerializer.Deserialize<TempReservation>(tempReservationJson);

            // Create user account if requested
            HotelGuests user = null;
            if (tempReservation.CreateAccount && !User.Identity.IsAuthenticated)
            {
                user = new HotelGuests
                {
                    UserName = tempReservation.GuestEmail,
                    Email = tempReservation.GuestEmail,
                    FirstName = tempReservation.FirstName,
                    LastName = tempReservation.LastName,
                    PhoneNumber = tempReservation.PhoneNumber,
                    Address = tempReservation.Address
                };

                var result = await _userManager.CreateAsync(user, "TempPassword123!");
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View("Payment", model);
                }

                // Optionally sign in the user
                // Replace the problematic line with the correct method call
                await _signInManager.SignInAsync(user, new AuthenticationProperties { IsPersistent = false });

            }
            else if (User.Identity.IsAuthenticated)
            {
                user = await _userManager.GetUserAsync(User);
            }

            // Create and save the reservation
            var reservation = new Data.GuestReservations
            {
                GuestId = user?.Id,
                RoomId = tempReservation.RoomId,
                CheckInDate = tempReservation.CheckInDate,
                CheckOutDate = tempReservation.CheckOutDate,
                NumberOfAdults = tempReservation.NumberOfAdults,
                NumberOfChildren = tempReservation.NumberOfChildren,
                TotalAmount = tempReservation.TotalPrice,
                BookingStatus = "Confirmed",
                SpecialRequests = tempReservation.SpecialRequests,
                PaymentStatus = "Paid",
                GuestEmail = tempReservation.GuestEmail,
                GuestName = $"{tempReservation.FirstName} {tempReservation.LastName}",
                GuestPhone = tempReservation.PhoneNumber,
                CreatedDate = DateTime.Now
            };

            _context.GuestReservations.Add(reservation);
            await _context.SaveChangesAsync();

            // Create and save payment
            var payment = new ReservationPayments
            {
                ReservationId = reservation.ReservationId,
                Amount = model.AmountPaid,
                PaymentDate = DateTime.Now,
                PaymentMethod = model.PaymentMethod,
                TransactionId = Guid.NewGuid().ToString(),
                Status = "Completed",
                Notes = model.Notes
            };

            _context.ReservationPayments.Add(payment);
            await _context.SaveChangesAsync();

            // Clear the temp reservation
            TempData.Remove("TempReservation");

            return RedirectToAction("Receipt", new { paymentId = payment.PaymentId });
        }

        [HttpGet]
        public IActionResult Receipt(int paymentId)
        {
            var payment = _context.ReservationPayments
                .Include(p => p.Reservation)
                .ThenInclude(r => r.Room)
                .ThenInclude(room => room.RoomType)
                .Include(p => p.Reservation)
                .ThenInclude(r => r.Guest)
                .FirstOrDefault(p => p.PaymentId == paymentId);

            if (payment == null)
            {
                return NotFound();
            }

            var model = new ReceiptViewModel
            {
                PaymentId = payment.PaymentId,
                PaymentDate = payment.PaymentDate,
                PaymentMethod = payment.PaymentMethod,
                TransactionId = payment.TransactionId,
                AmountPaid = payment.Amount,
                ReservationId = payment.Reservation.ReservationId,
                GuestName = payment.Reservation.Guest != null ?
                    $"{payment.Reservation.Guest.FirstName} {payment.Reservation.Guest.LastName}" :
                    payment.Reservation.GuestName,
                RoomType = payment.Reservation.Room.RoomType,
                RoomNumber = payment.Reservation.Room.RoomNumber,
                CheckInDate = payment.Reservation.CheckInDate,
                CheckOutDate = payment.Reservation.CheckOutDate,
                NumberOfNights = (int)(payment.Reservation.CheckOutDate - payment.Reservation.CheckInDate).TotalDays,
                BasePrice = payment.Reservation.Room.BasePrice,
                TotalAmount = payment.Reservation.TotalAmount
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ConfirmBooking(BookingViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("BookRoom", model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Create temporary reservation
            var tempReservation = new TempReservation
            {
                RoomTypeId = model.RoomTypeId,
                RoomId = model.RoomId,
                RoomNumber = model.RoomNumber,
                CheckInDate = model.CheckInDate,
                CheckOutDate = model.CheckOutDate,
                NumberOfAdults = model.NumberOfAdults,
                NumberOfChildren = model.NumberOfChildren,
                TotalPrice = model.TotalPrice,
                SpecialRequests = model.SpecialRequests,
                GuestEmail = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber
            };

            TempData["TempReservation"] = JsonSerializer.Serialize(tempReservation);

            return RedirectToAction("Payment");
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