using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RETRA_HotelSys.Data
{
    public class GuestReservations
    {
        [Key]
        public int ReservationId { get; set; }

        [ForeignKey("Guest")]
        public string GuestId { get; set; }  // Changed to string to match IdentityUser's Id type

        [ForeignKey("Room")]
        public int RoomId { get; set; }

        [Required]
        public DateTime CheckInDate { get; set; }

        [Required]
        public DateTime CheckOutDate { get; set; }

        [Required]
        [Range(1, 10)]
        public int NumberOfAdults { get; set; } = 1;

        [Range(0, 10)]
        public int NumberOfChildren { get; set; } = 0;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TaxAmount { get; set; } = 0;

        [StringLength(50)]
        public string BookingStatus { get; set; } = "Pending";

        public string? SpecialRequests { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ModifiedDate { get; set; }
        public DateTime? CancellationDate { get; set; }

        // Navigation properties
        public virtual HotelGuests Guest { get; set; }
        public virtual HotelRooms Room { get; set; }
        public virtual ICollection<ReservationStatusHistory> StatusHistory { get; set; } = new List<ReservationStatusHistory>();
        public virtual ICollection<ReservationPayments> Payments { get; set; } = new List<ReservationPayments>();
        public string PaymentStatus { get; internal set; }

        [NotMapped]
        public object ReservationStatus { get; internal set; }
        public bool IsWalkIn { get; internal set; }
        
        [StringLength(20)]
        public string GuestPhone { get; set; } // Make sure this is set

        [StringLength(100)]
        public string GuestName { get; set; }

        [StringLength(100)]
        public string GuestEmail { get; set; }
    }
}