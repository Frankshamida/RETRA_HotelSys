using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RETRA_HotelSys.Data
{
    public class HotelRooms
    {
        [Key]
        public int RoomId { get; set; }

        [Required]
        [StringLength(50)]
        public string RoomNumber { get; set; }

        [ForeignKey("RoomCategory")]
        public int RoomTypeId { get; set; }

        [Required]
        [StringLength(100)]
        public string RoomType { get; set; }

        public int FloorNumber { get; set; }

        [StringLength(50)]
        public string? ViewType { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        // Availability status (using boolean)
        public bool IsAvailable { get; set; } = true;

        // Clean status
        public bool IsClean { get; set; } = true;

        // Computed property for string status (not mapped to database)
        [NotMapped]
        public string Status => IsAvailable ? "Available" : "Occupied";

        [StringLength(1000)]
        public string? MaintenanceNotes { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? LastUpdated { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal BasePrice { get; set; }

        [Required]
        [Range(1, 10)]
        public int MaxOccupancy { get; set; }

        [StringLength(255)]
        public string? ImageUrl { get; set; }

        public DateTime? LastCleaned { get; set; }

        [StringLength(500)]
        public string? Amenities { get; set; }

        // Navigation properties
        public virtual RoomCategories RoomCategory { get; set; }
        public virtual ICollection<GuestReservations> GuestReservations { get; set; } = new List<GuestReservations>();
        public virtual ICollection<RoomMaintenance> RoomMaintenances { get; set; } = new List<RoomMaintenance>();
        public string Notes { get; internal set; }
        public DateTime ModifiedDate { get; internal set; }
        public bool IsActive { get; internal set; }
        public string RoomName { get; internal set; }
    }
}