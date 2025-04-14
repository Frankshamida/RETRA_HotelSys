using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RETRA_HotelSys.Data
{
    public class RoomCategories
    {
        [Key]
        public int RoomTypeId { get; set; }

        [Required]
        [StringLength(100)]
        public string TypeName { get; set; }

        public string? Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal BasePrice { get; set; }

        public int MaxCapacity { get; set; }
        public int? SizeInSqFt { get; set; }
        public string? BedConfiguration { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ModifiedDate { get; set; }

        // Navigation properties
        public virtual ICollection<HotelRooms> HotelRooms { get; set; } = new List<HotelRooms>();
        public virtual ICollection<RoomTypeFeatures> RoomTypeFeatures { get; set; } = new List<RoomTypeFeatures>();
    }
}