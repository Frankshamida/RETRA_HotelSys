using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using RETRA_HotelSys.Data;

namespace RETRA_HotelSys.Models.StaffManagement
{
    public class CreateStaffViewModel
    {
        [Required, StringLength(50, MinimumLength = 3)]
        public string Username { get; set; }

        [Required, StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, StringLength(50)]
        public string FirstName { get; set; }

        [Required, StringLength(50)]
        public string LastName { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        public int RoleId { get; set; }

        public List<StaffRoles> AvailableRoles { get; set; } = new List<StaffRoles>();
    }
}
