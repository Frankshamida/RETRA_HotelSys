using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using RETRA_HotelSys.Data;

namespace RETRA_HotelSys.Models.StaffManagement
{
    public class EditStaffViewModel
    {
        public int StaffId { get; set; }

        public string Username { get; set; }

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

        public bool IsActive { get; set; }

        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        public List<StaffRoles> AvailableRoles { get; set; }
    }
}
