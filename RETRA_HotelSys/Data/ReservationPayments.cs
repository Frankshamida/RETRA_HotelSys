using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RETRA_HotelSys.Data
{
    public class ReservationPayments
    {
        [Key]
        public int PaymentId { get; set; }

        [ForeignKey("Reservation")]
        public int ReservationId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.Now;

        [StringLength(50)]
        public string PaymentMethod { get; set; }

        [StringLength(50)]
        public string TransactionId { get; set; }

        public string? PaymentReference { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "Completed";

        [ForeignKey("ProcessedByStaff")]
        public int? ProcessedBy { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal RefundAmount { get; set; } = 0;

        public DateTime? RefundDate { get; set; }
        public string? Notes { get; set; }

        // Navigation properties
        public virtual GuestReservations Reservation { get; set; }
        public virtual StaffMembers? ProcessedByStaff { get; set; }
    }
}