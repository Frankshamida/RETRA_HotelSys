using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;
using RETRA_HotelSys.Data;

namespace RETRA_HotelSys.Models.Guest
{
    public class GuestReservations
    {
        public int ReservationId { get; set; }

        // Make GuestId nullable for non-registered guests
        public string? GuestId { get; set; }

        [ForeignKey("RoomId")]
        public HotelRooms Room { get; set; }
        public int RoomId { get; set; }

        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfAdults { get; set; }
        public int NumberOfChildren { get; set; }
        public decimal TotalAmount { get; set; }
        public string BookingStatus { get; set; } = "Pending"; // Default to Pending
        public string? SpecialRequests { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string PaymentStatus { get; set; } = "Pending"; // Default to Pending

        // For non-registered guests
        [StringLength(20)]
        public string GuestPhone { get; set; } // Make sure this is set

        [StringLength(100)]
        public string GuestName { get; set; }

        [StringLength(100)]
        public string GuestEmail { get; set; }

        // Navigation properties
        public HotelGuests? Guest { get; set; }
        public ICollection<ReservationPayments> Payments { get; set; } = new List<ReservationPayments>();
    }
}
