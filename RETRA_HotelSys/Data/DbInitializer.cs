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
            await SeedInitialDataAsync(context);
        }

        private static async Task SeedInitialDataAsync(ApplicationDbContext context)
        {
            if (!await context.StaffRoles.AnyAsync())
            {
                var staffRoles = new List<StaffRoles>
                {
                    new StaffRoles { RoleName = "Manager", Description = "Hotel Manager" },
                    new StaffRoles { RoleName = "Receptionist", Description = "Front Desk Staff" },
                    new StaffRoles { RoleName = "Housekeeper", Description = "Cleaning Staff" },
                    new StaffRoles { RoleName = "Maintenance", Description = "Maintenance Staff" }
                };

                await context.StaffRoles.AddRangeAsync(staffRoles);
                await context.SaveChangesAsync();
            }

            if (!await context.RoomCategories.AnyAsync())
            {
                var roomCategories = new List<RoomCategories>
                {
                    new RoomCategories {
                        TypeName = "Standard",
                        Description = "Standard Room with basic amenities",
                        BasePrice = 100.00m,
                        MaxCapacity = 2,
                        SizeInSqFt = 300,
                        BedConfiguration = "1 Queen Bed"
                    },
                    new RoomCategories {
                        TypeName = "Deluxe",
                        Description = "Deluxe Room with premium amenities",
                        BasePrice = 150.00m,
                        MaxCapacity = 2,
                        SizeInSqFt = 400,
                        BedConfiguration = "1 King Bed"
                    },
                    new RoomCategories {
                        TypeName = "Suite",
                        Description = "Spacious Suite with separate living area",
                        BasePrice = 250.00m,
                        MaxCapacity = 4,
                        SizeInSqFt = 600,
                        BedConfiguration = "1 King Bed + 1 Sofa Bed"
                    }
                };

                await context.RoomCategories.AddRangeAsync(roomCategories);
                await context.SaveChangesAsync();
            }
        }
    }
}