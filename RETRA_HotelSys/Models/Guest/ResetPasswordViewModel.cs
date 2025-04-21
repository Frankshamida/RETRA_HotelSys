using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace RETRA_HotelSys.Models.Guest
{
    public class ResetPasswordViewModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string OtpCode { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords don't match.")]
        public string ConfirmPassword { get; set; }
    }
}
