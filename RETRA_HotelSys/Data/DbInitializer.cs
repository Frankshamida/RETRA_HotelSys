using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RETRA_HotelSys.Data;
using System;
using System.Threading.Tasks;

namespace RETRA_HotelSys
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(
            ApplicationDbContext context,
            UserManager<HotelGuests> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            // Create database if it doesn't exist
            await context.Database.EnsureCreatedAsync();

            // Create roles if they don't exist
            string[] roleNames = { "Admin", "Guest", "FrontDesk", "Housekeeping" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Create StaffRoles (different from Identity roles)
            if (!await context.StaffRoles.AnyAsync())
            {
                var staffRoles = new List<StaffRoles>
        {
            new StaffRoles { RoleName = "Manager", Description = "Hotel Manager" },
            new StaffRoles { RoleName = "Receptionist", Description = "Front Desk Staff" },
            new StaffRoles { RoleName = "Housekeeper", Description = "Cleaning Staff" }
        };
                await context.StaffRoles.AddRangeAsync(staffRoles);
                await context.SaveChangesAsync();
            }

            // Create admin user if it doesn't exist
            var adminEmail = "admin@retra.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new HotelGuests
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Admin",
                    LastName = "User",
                    PhoneNumber = "1234567890",
                    IsActive = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // Seed initial data if needed

        }
    }
}