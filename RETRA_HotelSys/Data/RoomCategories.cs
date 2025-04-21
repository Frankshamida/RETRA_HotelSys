using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RETRA_HotelSys.Data
{
    // Add this to your Data namespace
    public class RoomCategories
    {
        [Key]
        public int RoomTypeId { get; set; }

        [Required]
        [StringLength(100)]
        public string TypeName { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal BasePrice { get; set; }

        public int MaxCapacity { get; set; }

        public int? SizeInSqFt { get; set; }

        [StringLength(100)]
        public string BedConfiguration { get; set; }

        [StringLength(500)]
        public string MainImagePath { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedDate { get; set; }

        // Navigation properties
        public virtual ICollection<HotelRooms> HotelRooms { get; set; } = new List<HotelRooms>();
        public virtual ICollection<RoomTypeFeatures> RoomTypeFeatures { get; set; } = new List<RoomTypeFeatures>();
    }
}