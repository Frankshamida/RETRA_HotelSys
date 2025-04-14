using System.ComponentModel.DataAnnotations.Schema;

namespace RETRA_HotelSys.Data
{
    public class RoomTypeFeatures
    {
        [ForeignKey("RoomCategory")]
        public int RoomTypeId { get; set; }

        [ForeignKey("RoomFeature")]
        public int AmenityId { get; set; }

        public bool IsIncluded { get; set; } = true;

        [Column(TypeName = "decimal(18,2)")]
        public decimal AdditionalCost { get; set; } = 0;

        // Navigation properties
        public virtual RoomCategories RoomCategory { get; set; }
        public virtual RoomFeatures RoomFeature { get; set; }
    }
}