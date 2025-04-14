using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace RETRA_HotelSys.Models
{
    public class AdminAccessCodeViewModel
    {
        [Required]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Access code must be 6 digits")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Access code must contain only numbers")]
        public string AccessCode { get; set; }

        public string ReturnUrl { get; set; }
    }
}
