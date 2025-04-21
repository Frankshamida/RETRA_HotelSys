using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace RETRA_HotelSys.Models.Guest
{
    public class PaymentViewModel
    {
        public int ReservationId { get; set; }

        [Display(Name = "Amount Due")]
        public decimal AmountDue { get; set; }

        [Required]
        [Display(Name = "Amount Paid")]
        public decimal AmountPaid { get; set; }

        [Required]
        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; }

        [Display(Name = "Notes")]
        public string Notes { get; set; }

        // For display only
        public string RoomType { get; set; }
        public string RoomNumber { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public string GuestName { get; set; }
        public int RoomId { get; internal set; }
        public int NumberOfAdults { get; internal set; }
        public int NumberOfChildren { get; internal set; }
        public string SpecialRequests { get; internal set; }
    }
}
