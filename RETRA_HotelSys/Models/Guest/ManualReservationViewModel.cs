using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using RETRA_HotelSys.Data;

namespace RETRA_HotelSys.Models.Guest
{
    public class ManualReservationViewModel
    {
        // Guest Information
        [Required(ErrorMessage = "First name is required")]
        public string GuestFirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        public string GuestLastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string GuestEmail { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        public string GuestPhone { get; set; }

        // Reservation Information
        [Required(ErrorMessage = "Please select a room")]
        public int RoomId { get; set; }

        [Required(ErrorMessage = "Check-in date is required")]
        public DateTime CheckInDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Check-out date is required")]
        public DateTime CheckOutDate { get; set; } = DateTime.Today.AddDays(1);

        [Range(1, 10, ErrorMessage = "At least 1 adult required")]
        public int NumberOfAdults { get; set; } = 1;

        [Range(0, 10, ErrorMessage = "Children must be between 0 and 10")]
        public int NumberOfChildren { get; set; }

        public string SpecialRequests { get; set; }

        [Required]
        public string BookingStatus { get; set; } = "Confirmed";

        [ValidateNever]
        public List<HotelRooms> AvailableRooms { get; set; }

        public decimal TotalAmount { get; set; }


        [Required]
        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; }

        [Display(Name = "Transaction Reference")]
        public string TransactionReference { get; set; }

        [Display(Name = "Payment Amount")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal PaymentAmount { get; set; }

        [Display(Name = "Payment Notes")]
        public string PaymentNotes { get; set; }
    }
}

