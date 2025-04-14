using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RETRA_HotelSys.Data
{
    public class ReservationStatusHistory
    {
        [Key]
        public int HistoryId { get; set; }

        [ForeignKey("Reservation")]
        public int ReservationId { get; set; }

        [StringLength(50)]
        public string Status { get; set; }

        [ForeignKey("ChangedByStaff")]
        public int? ChangedBy { get; set; }

        public DateTime ChangeDate { get; set; } = DateTime.Now;

        [StringLength(255)]
        public string Notes { get; set; }
        public bool IsSystemGenerated { get; set; } = false;

        // Navigation properties
        public virtual GuestReservations Reservation { get; set; }
        public virtual StaffMembers? ChangedByStaff { get; set; }
    }
}