using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RETRA_HotelSys.Data;
using RETRA_HotelSys.Models;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using RETRA_HotelSys.Models.StaffManagement;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Rendering;
using RETRA_HotelSys.Models.RoomManagement;
using Microsoft.AspNetCore.Hosting;

[Authorize(Roles = "Admin")] // Restrict entire controller to Admin role only
[Route("Admin")] // Set base route for all actions
[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)] // Disable caching globally for this controller
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<HotelGuests> _userManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AdminController> _logger;
    private readonly IWebHostEnvironment _hostingEnvironment;

    public AdminController(
        ApplicationDbContext context,
        UserManager<HotelGuests> userManager,
        IConfiguration configuration,
        ILogger<AdminController> logger,
        IWebHostEnvironment hostingEnvironment)
    {
        _context = context;
        _userManager = userManager;
        _configuration = configuration;
        _logger = logger;
        _hostingEnvironment = hostingEnvironment;
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


    [HttpGet("StaffManagement")]
    public async Task<IActionResult> StaffManagement()
    {
        var staffList = await _context.StaffMembers
            .Include(s => s.StaffRole)
            .OrderBy(s => s.LastName)
            .ToListAsync();

        return View(new StaffManagementViewModel
        {
            StaffMembers = staffList,
            AvailableRoles = await _context.StaffRoles.ToListAsync()
        });
    }

    [HttpGet("StaffManagement/Create")]
    public async Task<IActionResult> CreateStaff()
    {
        var roles = await _context.StaffRoles.ToListAsync();
        if (!roles.Any())
        {
            TempData["ErrorMessage"] = "No roles available. Please create roles first.";
            return RedirectToAction("StaffManagement");
        }

        return View(new CreateStaffViewModel
        {
            AvailableRoles = roles
        });
    }

    [HttpPost("StaffManagement/Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateStaff(CreateStaffViewModel model)
    {
        try
        {
            // Always load available roles
            model.AvailableRoles = await _context.StaffRoles.ToListAsync();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Verify role exists
            var roleExists = await _context.StaffRoles.AnyAsync(r => r.RoleId == model.RoleId);
            if (!roleExists)
            {
                ModelState.AddModelError("RoleId", "Selected role is invalid");
                return View(model);
            }

            // Check for duplicates
            if (await _context.StaffMembers.AnyAsync(s => s.Username == model.Username))
            {
                ModelState.AddModelError("Username", "Username already exists");
                return View(model);
            }

            if (await _context.StaffMembers.AnyAsync(s => s.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Email already exists");
                return View(model);
            }

            // Create new staff
            var staff = new StaffMembers
            {
                Username = model.Username.Trim(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Email = model.Email.Trim(),
                FirstName = model.FirstName.Trim(),
                LastName = model.LastName.Trim(),
                PhoneNumber = model.PhoneNumber?.Trim(),
                RoleId = model.RoleId,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };

            _context.StaffMembers.Add(staff);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Staff member {staff.Username} created successfully!";
            return RedirectToAction("StaffManagement");
        }
        catch (DbUpdateException dbEx)
        {
            _logger.LogError(dbEx, "Database error creating staff");
            TempData["ErrorMessage"] = "Database error occurred. Please try again.";
            model.AvailableRoles = await _context.StaffRoles.ToListAsync();
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error creating staff");
            TempData["ErrorMessage"] = "An unexpected error occurred";
            model.AvailableRoles = await _context.StaffRoles.ToListAsync();
            return View(model);
        }
    }

    // Edit Staff - GET
    [HttpGet("StaffManagement/Edit/{id}")]
    public async Task<IActionResult> EditStaff(int id)
    {
        try
        {
            var staff = await _context.StaffMembers.FindAsync(id);
            if (staff == null)
            {
                TempData["ErrorMessage"] = "Staff member not found";
                return RedirectToAction("StaffManagement");
            }

            return View(new EditStaffViewModel
            {
                StaffId = staff.StaffId,
                Username = staff.Username,
                Email = staff.Email,
                FirstName = staff.FirstName,
                LastName = staff.LastName,
                PhoneNumber = staff.PhoneNumber,
                RoleId = staff.RoleId,
                IsActive = staff.IsActive,
                AvailableRoles = await _context.StaffRoles.ToListAsync()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to load staff editor for ID {id}");
            TempData["ErrorMessage"] = "Failed to load staff editor";
            return RedirectToAction("StaffManagement");
        }
    }

    [HttpPost("StaffManagement/Edit/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditStaff(int id, EditStaffViewModel model)
    {
        _logger.LogInformation($"EditStaff POST called for ID: {id} with model: {JsonSerializer.Serialize(model)}");

        try
        {
            // Log model state before validation
            _logger.LogInformation($"ModelState IsValid: {ModelState.IsValid}");

            if (!ModelState.IsValid)
            {
                var errors = string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));

                _logger.LogWarning($"Validation errors: {errors}");

                // Ensure available roles are populated before returning
                model.AvailableRoles = await _context.StaffRoles.ToListAsync();
                _logger.LogInformation($"Returning to view with {model.AvailableRoles.Count} roles available");
                return View(model);
            }

            // Verify the ID in route matches the model
            if (id != model.StaffId)
            {
                _logger.LogWarning($"ID mismatch: Route ID {id} vs Model ID {model.StaffId}");
                TempData["ErrorMessage"] = "Staff ID mismatch";
                return RedirectToAction("StaffManagement");
            }

            var staff = await _context.StaffMembers.FindAsync(id);
            if (staff == null)
            {
                _logger.LogWarning($"Staff member with ID {id} not found");
                TempData["ErrorMessage"] = "Staff member not found";
                return RedirectToAction("StaffManagement");
            }

            // Verify role exists
            var roleExists = await _context.StaffRoles.AnyAsync(r => r.RoleId == model.RoleId);
            if (!roleExists)
            {
                _logger.LogWarning($"Invalid role ID {model.RoleId} selected");
                ModelState.AddModelError("RoleId", "Selected role is invalid");
                model.AvailableRoles = await _context.StaffRoles.ToListAsync();
                return View(model);
            }

            // Check for email conflicts (excluding current staff)
            if (await _context.StaffMembers.AnyAsync(s => s.Email == model.Email && s.StaffId != id))
            {
                _logger.LogWarning($"Email {model.Email} already in use by another staff member");
                ModelState.AddModelError("Email", "Email already in use by another staff member");
                model.AvailableRoles = await _context.StaffRoles.ToListAsync();
                return View(model);
            }

            // Log changes
            _logger.LogInformation($"Updating staff {id} from: " +
                $"Email={staff.Email}, " +
                $"FirstName={staff.FirstName}, " +
                $"LastName={staff.LastName}, " +
                $"Phone={staff.PhoneNumber}, " +
                $"Role={staff.RoleId}, " +
                $"Active={staff.IsActive}");

            _logger.LogInformation($"Updating staff {id} to: " +
                $"Email={model.Email}, " +
                $"FirstName={model.FirstName}, " +
                $"LastName={model.LastName}, " +
                $"Phone={model.PhoneNumber}, " +
                $"Role={model.RoleId}, " +
                $"Active={model.IsActive}");

            // Update properties
            staff.Email = model.Email.Trim();
            staff.FirstName = model.FirstName.Trim();
            staff.LastName = model.LastName.Trim();
            staff.PhoneNumber = model.PhoneNumber?.Trim();
            staff.RoleId = model.RoleId;
            staff.IsActive = model.IsActive;
            staff.ModifiedDate = DateTime.UtcNow;

            // Only update password if provided
            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                _logger.LogInformation("Updating password for staff member");
                staff.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation($"Staff member {id} updated successfully");

            TempData["SuccessMessage"] = "Staff member updated successfully!";
            return RedirectToAction("StaffManagement");
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogError(ex, $"Concurrency error updating staff member ID {id}");
            TempData["ErrorMessage"] = "The record was modified by another user. Please refresh and try again.";
        }
        catch (DbUpdateException dbEx)
        {
            _logger.LogError(dbEx, $"Database error updating staff member ID {id}");
            TempData["ErrorMessage"] = "Database error occurred while updating staff member.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Unexpected error updating staff member ID {id}");
            TempData["ErrorMessage"] = "An unexpected error occurred while updating staff member.";
        }

        // If we got here, something went wrong - reload the view
        model.AvailableRoles = await _context.StaffRoles.ToListAsync();
        return View(model);
    }

    [HttpPost("StaffManagement/Delete/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteStaff(int id)
    {
        try
        {
            var staff = await _context.StaffMembers.FindAsync(id);
            if (staff == null)
            {
                TempData["ErrorMessage"] = "Staff member not found";
                return RedirectToAction("StaffManagement");
            }

            _context.StaffMembers.Remove(staff);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Staff member deleted successfully!";
            return RedirectToAction("StaffManagement");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to delete staff member ID {id}");
            TempData["ErrorMessage"] = "Failed to delete staff member. They may be associated with existing records.";
            return RedirectToAction("StaffManagement");
        }
    }

    [HttpGet("RoomManagement")]
    public async Task<IActionResult> RoomManagement()
    {
        var model = new RoomManagementViewModel
        {
            RoomCategories = await _context.RoomCategories
                .Include(rc => rc.HotelRooms)
                .Include(rc => rc.RoomTypeFeatures)
                .ThenInclude(rtf => rtf.RoomFeature)
                .OrderBy(rc => rc.TypeName)
                .ToListAsync(),

            AllFeatures = await _context.RoomFeatures.ToListAsync(),

            RoomStatusSummary = new RoomStatusSummary
            {
                TotalRooms = await _context.HotelRooms.CountAsync(),
                AvailableRooms = await _context.HotelRooms.CountAsync(r => r.IsAvailable),
                OccupiedRooms = await _context.HotelRooms.CountAsync(r => !r.IsAvailable),
                MaintenanceRooms = await _context.HotelRooms
                    .CountAsync(r => _context.RoomMaintenance
                        .Any(rm => rm.RoomId == r.RoomId && rm.Status != "Completed"))
            }
        };

        return View(model);
    }

    [HttpGet("RoomManagement/CreateCategory")]
    public async Task<IActionResult> CreateRoomCategory()
    {
        var model = new CreateRoomCategoryViewModel
        {
            AllFeatures = await _context.RoomFeatures.ToListAsync()
        };
        return View(model);
    }

    [HttpPost("RoomManagement/CreateCategory")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateRoomCategory(CreateRoomCategoryViewModel model)
    {
        try
        {
            model.AllFeatures = await _context.RoomFeatures.ToListAsync();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Check for duplicate category name
            if (await _context.RoomCategories.AnyAsync(rc => rc.TypeName.ToLower() == model.TypeName.ToLower()))
            {
                ModelState.AddModelError("TypeName", "A room category with this name already exists");
                return View(model);
            }

            var roomCategory = new RoomCategories
            {
                TypeName = model.TypeName,
                Description = model.Description,
                BasePrice = model.BasePrice,
                MaxCapacity = model.MaxCapacity,
                SizeInSqFt = model.SizeInSqFt,
                BedConfiguration = model.BedConfiguration,
                MainImagePath = model.MainImagePath,
                CreatedDate = DateTime.UtcNow
            };

            _context.RoomCategories.Add(roomCategory);
            await _context.SaveChangesAsync(); // Save first to get the ID

            // Add selected features
            if (model.SelectedFeatureIds != null && model.SelectedFeatureIds.Any())
            {
                foreach (var featureId in model.SelectedFeatureIds)
                {
                    var feature = await _context.RoomFeatures.FindAsync(featureId);
                    if (feature != null)
                    {
                        _context.RoomTypeFeatures.Add(new RoomTypeFeatures
                        {
                            RoomTypeId = roomCategory.RoomTypeId,
                            AmenityId = featureId,
                            AdditionalCost = feature.DefaultAdditionalCost
                        });
                    }
                }
                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = "Room category created successfully!";
            return RedirectToAction("RoomManagement");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating room category");
            TempData["ErrorMessage"] = "An error occurred while creating the room category";
            model.AllFeatures = await _context.RoomFeatures.ToListAsync();
            return View(model);
        }
    }

    [HttpGet("RoomManagement/EditCategory/{id}")]
    public async Task<IActionResult> EditRoomCategory(int id)
    {
        var category = await _context.RoomCategories
            .Include(rc => rc.RoomTypeFeatures)
            .FirstOrDefaultAsync(rc => rc.RoomTypeId == id);

        if (category == null)
        {
            TempData["ErrorMessage"] = "Room category not found";
            return RedirectToAction("RoomManagement");
        }

        var model = new EditRoomCategoryViewModel
        {
            RoomTypeId = category.RoomTypeId,
            TypeName = category.TypeName,
            Description = category.Description,
            BasePrice = category.BasePrice,
            MaxCapacity = category.MaxCapacity,
            SizeInSqFt = category.SizeInSqFt,
            BedConfiguration = category.BedConfiguration,
            MainImagePath = category.MainImagePath,
            AllFeatures = await _context.RoomFeatures.ToListAsync(),
            SelectedFeatureIds = category.RoomTypeFeatures.Select(rtf => rtf.AmenityId).ToList()
        };

        return View(model);
    }

    [HttpPost("RoomManagement/EditCategory/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditRoomCategory(int id, EditRoomCategoryViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                model.AllFeatures = await _context.RoomFeatures.ToListAsync();
                return View(model);
            }

            var category = await _context.RoomCategories
                .Include(rc => rc.RoomTypeFeatures)
                .FirstOrDefaultAsync(rc => rc.RoomTypeId == id);

            if (category == null)
            {
                TempData["ErrorMessage"] = "Room category not found";
                return RedirectToAction("RoomManagement");
            }

            // Check for duplicate category name (excluding current category)
            if (await _context.RoomCategories.AnyAsync(rc => rc.TypeName == model.TypeName && rc.RoomTypeId != id))
            {
                ModelState.AddModelError("TypeName", "A room category with this name already exists");
                model.AllFeatures = await _context.RoomFeatures.ToListAsync();
                return View(model);
            }

            // Update category properties
            category.TypeName = model.TypeName;
            category.Description = model.Description;
            category.BasePrice = model.BasePrice;
            category.MaxCapacity = model.MaxCapacity;
            category.SizeInSqFt = model.SizeInSqFt;
            category.BedConfiguration = model.BedConfiguration;
            category.MainImagePath = model.MainImagePath;
            category.ModifiedDate = DateTime.UtcNow;

            // Update features
            var currentFeatures = category.RoomTypeFeatures.Select(rtf => rtf.AmenityId).ToList();
            var selectedFeatures = model.SelectedFeatureIds ?? new List<int>();

            // Remove features that were deselected
            var featuresToRemove = currentFeatures.Except(selectedFeatures).ToList();
            foreach (var featureId in featuresToRemove)
            {
                var featureToRemove = category.RoomTypeFeatures.FirstOrDefault(rtf => rtf.AmenityId == featureId);
                if (featureToRemove != null)
                {
                    _context.RoomTypeFeatures.Remove(featureToRemove);
                }
            }

            // Add new features that were selected
            var featuresToAdd = selectedFeatures.Except(currentFeatures).ToList();
            foreach (var featureId in featuresToAdd)
            {
                var feature = await _context.RoomFeatures.FindAsync(featureId);
                if (feature != null)
                {
                    _context.RoomTypeFeatures.Add(new RoomTypeFeatures
                    {
                        RoomTypeId = id,
                        AmenityId = featureId,
                        AdditionalCost = feature.DefaultAdditionalCost
                    });
                }
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Room category updated successfully!";
            return RedirectToAction("RoomManagement");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error editing room category ID {id}");
            TempData["ErrorMessage"] = "An error occurred while updating the room category";
            model.AllFeatures = await _context.RoomFeatures.ToListAsync();
            return View(model);
        }
    }

    [HttpPost("RoomManagement/DeleteCategory/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteRoomCategory(int id)
    {
        try
        {
            var category = await _context.RoomCategories
                .Include(rc => rc.HotelRooms)
                .FirstOrDefaultAsync(rc => rc.RoomTypeId == id);

            if (category == null)
            {
                TempData["ErrorMessage"] = "Room category not found";
                return RedirectToAction("RoomManagement");
            }

            // Check if category has rooms assigned
            if (category.HotelRooms.Any())
            {
                TempData["ErrorMessage"] = "Cannot delete category with assigned rooms. Please reassign or delete the rooms first.";
                return RedirectToAction("RoomManagement");
            }

            // Remove all features associated with this category
            var featuresToRemove = await _context.RoomTypeFeatures
                .Where(rtf => rtf.RoomTypeId == id)
                .ToListAsync();

            _context.RoomTypeFeatures.RemoveRange(featuresToRemove);
            _context.RoomCategories.Remove(category);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Room category deleted successfully!";
            return RedirectToAction("RoomManagement");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting room category ID {id}");
            TempData["ErrorMessage"] = "An error occurred while deleting the room category";
            return RedirectToAction("RoomManagement");
        }
    }

    [HttpGet("RoomManagement/RoomsByCategory/{categoryId}")]
    public async Task<IActionResult> RoomsByCategory(int categoryId)
    {
        var category = await _context.RoomCategories.FindAsync(categoryId);
        if (category == null)
        {
            TempData["ErrorMessage"] = "Room category not found";
            return RedirectToAction("RoomManagement");
        }

        var rooms = await _context.HotelRooms
            .Where(r => r.RoomTypeId == categoryId)
            .OrderBy(r => r.RoomNumber)
            .ToListAsync();

        var model = new RoomsByCategoryViewModel
        {
            Category = category,
            Rooms = rooms
        };

        return View(model);
    }

    [HttpGet("RoomManagement/CreateRoom/{categoryId}")]
    public async Task<IActionResult> CreateRoom(int categoryId)
    {
        var category = await _context.RoomCategories.FindAsync(categoryId);
        if (category == null)
        {
            TempData["ErrorMessage"] = "Room category not found";
            return RedirectToAction("RoomManagement");
        }

        var model = new CreateRoomViewModel
        {
            RoomTypeId = categoryId,
            CategoryName = category.TypeName,
            FloorOptions = Enumerable.Range(1, 20).Select(i => new SelectListItem
            {
                Value = i.ToString(),
                Text = i.ToString()
            }).ToList()
        };

        return View(model);
    }

    [HttpPost("RoomManagement/CreateRoom/{categoryId}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateRoom(int categoryId, CreateRoomViewModel model)
    {
        try
        {
            var category = await _context.RoomCategories.FindAsync(categoryId);
            if (category == null)
            {
                TempData["ErrorMessage"] = "Room category not found";
                return RedirectToAction("RoomManagement");
            }

            model.FloorOptions = Enumerable.Range(1, 20).Select(i => new SelectListItem
            {
                Value = i.ToString(),
                Text = i.ToString()
            }).ToList();

            if (!ModelState.IsValid)
            {
                model.CategoryName = category.TypeName;
                return View(model);
            }

            if (await _context.HotelRooms.AnyAsync(r =>
                r.RoomNumber.ToLower() == model.RoomNumber.ToLower()))
            {
                ModelState.AddModelError("RoomNumber", "A room with this number already exists");
                model.CategoryName = category.TypeName;
                return View(model);
            }

            var room = new HotelRooms
            {
                RoomNumber = model.RoomNumber,
                FloorNumber = model.FloorNumber,
                RoomTypeId = categoryId,
                RoomType = category.TypeName,
                Description = $"Room {model.RoomNumber} of type {category.TypeName}",
                IsAvailable = true,
                IsClean = true,
                BasePrice = category.BasePrice,
                MaxOccupancy = category.MaxCapacity,
                Notes = model.Notes,
                CreatedDate = DateTime.UtcNow
            };

            _context.HotelRooms.Add(room);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Room {model.RoomNumber} created successfully!";
            return RedirectToAction("RoomsByCategory", new { categoryId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error creating room in category ID {categoryId}");
            TempData["ErrorMessage"] = "An error occurred while creating the room";
            model.CategoryName = (await _context.RoomCategories.FindAsync(categoryId))?.TypeName;
            model.FloorOptions = Enumerable.Range(1, 20).Select(i => new SelectListItem
            {
                Value = i.ToString(),
                Text = i.ToString()
            }).ToList();
            return View(model);
        }
    }

    [HttpGet("RoomManagement/EditRoom/{id}")]
    public async Task<IActionResult> EditRoom(int id)
    {
        var room = await _context.HotelRooms
            .Include(r => r.RoomCategory)
            .FirstOrDefaultAsync(r => r.RoomId == id);

        if (room == null)
        {
            TempData["ErrorMessage"] = "Room not found";
            return RedirectToAction("RoomManagement");
        }

        var model = new EditRoomViewModel
        {
            RoomId = room.RoomId,
            RoomNumber = room.RoomNumber,
            FloorNumber = room.FloorNumber,
            RoomTypeId = room.RoomTypeId,
            CategoryName = room.RoomCategory.TypeName,
            IsAvailable = room.IsAvailable,
            IsClean = room.IsClean,
            Notes = room.Notes,
            FloorOptions = Enumerable.Range(1, 20).Select(i => new SelectListItem
            {
                Value = i.ToString(),
                Text = i.ToString(),
                Selected = i == room.FloorNumber
            }).ToList()
        };

        return View(model);
    }

    [HttpPost("RoomManagement/EditRoom/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditRoom(int id, EditRoomViewModel model)
    {
        try
        {
            var room = await _context.HotelRooms.FindAsync(id);
            if (room == null)
            {
                TempData["ErrorMessage"] = "Room not found";
                return RedirectToAction("RoomManagement");
            }

            if (!ModelState.IsValid)
            {
                model.CategoryName = (await _context.RoomCategories.FindAsync(room.RoomTypeId))?.TypeName;
                model.FloorOptions = Enumerable.Range(1, 20).Select(i => new SelectListItem
                {
                    Value = i.ToString(),
                    Text = i.ToString(),
                    Selected = i == model.FloorNumber
                }).ToList();
                return View(model);
            }

            // Check for duplicate room number (excluding current room)
            if (await _context.HotelRooms.AnyAsync(r => r.RoomNumber == model.RoomNumber && r.RoomId != id))
            {
                ModelState.AddModelError("RoomNumber", "A room with this number already exists");
                model.CategoryName = (await _context.RoomCategories.FindAsync(room.RoomTypeId))?.TypeName;
                model.FloorOptions = Enumerable.Range(1, 20).Select(i => new SelectListItem
                {
                    Value = i.ToString(),
                    Text = i.ToString(),
                    Selected = i == model.FloorNumber
                }).ToList();
                return View(model);
            }

            // Update room properties
            room.RoomNumber = model.RoomNumber;
            room.FloorNumber = model.FloorNumber;
            room.IsAvailable = model.IsAvailable;
            room.IsClean = model.IsClean;
            room.Notes = model.Notes;
            room.ModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Room updated successfully!";
            return RedirectToAction("RoomsByCategory", new { categoryId = room.RoomTypeId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error editing room ID {id}");
            TempData["ErrorMessage"] = "An error occurred while updating the room";
            model.CategoryName = (await _context.RoomCategories.FindAsync(model.RoomTypeId))?.TypeName;
            model.FloorOptions = Enumerable.Range(1, 20).Select(i => new SelectListItem
            {
                Value = i.ToString(),
                Text = i.ToString(),
                Selected = i == model.FloorNumber
            }).ToList();
            return View(model);
        }
    }

    [HttpPost("RoomManagement/DeleteRoom/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteRoom(int id)
    {
        try
        {
            var room = await _context.HotelRooms
                .Include(r => r.GuestReservations)
                .FirstOrDefaultAsync(r => r.RoomId == id);

            if (room == null)
            {
                TempData["ErrorMessage"] = "Room not found";
                return RedirectToAction("RoomManagement");
            }

            // Check if room has active reservations
            if (room.GuestReservations.Any(r => r.BookingStatus == "Confirmed" && r.CheckOutDate > DateTime.Now))
            {
                TempData["ErrorMessage"] = "Cannot delete room with active reservations";
                return RedirectToAction("RoomsByCategory", new { categoryId = room.RoomTypeId });
            }

            // Soft delete by marking as unavailable
            room.IsAvailable = false;
            room.IsActive = false;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Room deleted successfully!";
            return RedirectToAction("RoomsByCategory", new { categoryId = room.RoomTypeId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting room ID {id}");
            TempData["ErrorMessage"] = "An error occurred while deleting the room";
            return RedirectToAction("RoomManagement");
        }
    }

    [HttpGet("RoomManagement/RoomDetails/{id}")]
    public async Task<IActionResult> RoomDetails(int id)
    {
        var room = await _context.HotelRooms
            .Include(r => r.RoomCategory)
            .ThenInclude(rc => rc.RoomTypeFeatures)
            .ThenInclude(rtf => rtf.RoomFeature)
            .Include(r => r.GuestReservations)
                .ThenInclude(gr => gr.Guest)
            .Include(r => r.RoomMaintenances)
            .FirstOrDefaultAsync(r => r.RoomId == id);

        if (room == null)
        {
            TempData["ErrorMessage"] = "Room not found";
            return RedirectToAction("RoomManagement");
        }

        // Get current reservation if any
        var currentReservation = room.GuestReservations
            .FirstOrDefault(r => r.BookingStatus == "Confirmed" &&
                               r.CheckInDate <= DateTime.Now &&
                               r.CheckOutDate >= DateTime.Now);

        var model = new RoomDetailsViewModel
        {
            Room = room,
            CurrentReservation = currentReservation != null ?
                new ReservationViewModel
                {
                    GuestName = $"{currentReservation.Guest.FirstName} {currentReservation.Guest.LastName}",
                    CheckInDate = currentReservation.CheckInDate,
                    CheckOutDate = currentReservation.CheckOutDate,
                    BookingStatus = currentReservation.BookingStatus
                } : null,
            UpcomingReservations = room.GuestReservations
                .Where(r => r.BookingStatus == "Confirmed" && r.CheckInDate > DateTime.Now)
                .OrderBy(r => r.CheckInDate)
                .Take(5)
                .Select(r => new ReservationViewModel
                {
                    GuestName = $"{r.Guest.FirstName} {r.Guest.LastName}",
                    CheckInDate = r.CheckInDate,
                    CheckOutDate = r.CheckOutDate,
                    BookingStatus = r.BookingStatus
                }).ToList(),
            MaintenanceHistory = room.RoomMaintenances
                .OrderByDescending(m => m.ScheduledDate)
                .Take(5)
                .Select(m => new MaintenanceViewModel
                {
                    ScheduledDate = m.ScheduledDate,
                    Type = m.TaskType, // Changed from m.Type to m.TaskType
                    Status = m.Status
                }).ToList(),
            RoomFeatures = room.RoomCategory.RoomTypeFeatures
                .Select(rtf => rtf.RoomFeature)
                .ToList()
        };

        return View(model);
    }

    // Room number verification endpoint
    [HttpGet("VerifyRoomNumber")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyRoomNumber(string roomNumber, int roomTypeId)
    {
        var exists = await _context.HotelRooms
            .AnyAsync(r => r.RoomNumber.ToLower() == roomNumber.ToLower() &&
                          r.RoomTypeId == roomTypeId);

        return Json(!exists);
    }

    // Image upload endpoint
    [HttpPost("UploadImage")]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { success = false, message = "No file uploaded" });
            }

            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
            {
                return BadRequest(new { success = false, message = "Invalid file type. Only JPG, JPEG, PNG, and GIF are allowed." });
            }

            // Validate file size (5MB limit)
            if (file.Length > 5 * 1024 * 1024)
            {
                return BadRequest(new { success = false, message = "File size exceeds the 5MB limit." });
            }

            var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok(new
            {
                success = true,
                filePath = $"/uploads/{uniqueFileName}",
                fileName = uniqueFileName
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading image");
            return StatusCode(500, new { success = false, message = "An error occurred while uploading the file." });
        }
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