using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RETRA_HotelSys.Data
{
    public class RoomFeatures
    {
        [Key]
        public int AmenityId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(255)]
        public string Description { get; set; }

        [MaxLength(50)]
        public string IconClass { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DefaultAdditionalCost { get; set; }

        [NotMapped]
        public bool IsSelected { get; set; }

        // Navigation property
        public virtual ICollection<RoomTypeFeatures> RoomTypeFeatures { get; set; } = new List<RoomTypeFeatures>();
        public string FeatureName { get; internal set; }
    }
}