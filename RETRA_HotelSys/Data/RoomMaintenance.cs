using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RETRA_HotelSys.Data
{
    public class RoomMaintenance
    {
        [Key]
        public int TaskId { get; set; }

        [ForeignKey("HotelRoom")]
        public int RoomId { get; set; }

        [ForeignKey("AssignedStaff")]
        public int? StaffId { get; set; }

        [Required]
        [StringLength(50)]
        public string TaskType { get; set; } = "Cleaning";

        [Required]
        public DateTime ScheduledDate { get; set; }

        public DateTime? StartTime { get; set; }
        public DateTime? CompletionTime { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Pending";

        public int Priority { get; set; } = 3;
        public string? Notes { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [ForeignKey("CreatedByStaff")]
        public int? CreatedBy { get; set; }

        // Navigation properties
        public virtual HotelRooms HotelRoom { get; set; }
        public virtual StaffMembers? AssignedStaff { get; set; }
        public virtual StaffMembers? CreatedByStaff { get; set; }
    }
}