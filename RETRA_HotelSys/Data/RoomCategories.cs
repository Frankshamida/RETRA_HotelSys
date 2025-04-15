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

        // Image properties
        [StringLength(255)]
        public string? MainImagePath { get; set; } // Path to the main room image

        // For multiple images (consider a separate table if you need many images per room type)
        public string? AdditionalImagesJson { get; set; } // JSON array of image paths, or consider a separate RoomTypeImages table

        // Navigation properties
        public virtual ICollection<HotelRooms> HotelRooms { get; set; } = new List<HotelRooms>();
        public virtual ICollection<RoomTypeFeatures> RoomTypeFeatures { get; set; } = new List<RoomTypeFeatures>();
    }
}