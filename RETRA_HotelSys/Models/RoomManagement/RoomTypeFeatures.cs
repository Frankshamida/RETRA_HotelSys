using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using RETRA_HotelSys.Data;

namespace RETRA_HotelSys.Models.RoomManagement
{
    public class RoomTypeFeatures
    {
        [Key, Column(Order = 0)]
        public int RoomTypeId { get; set; }

        [Key, Column(Order = 1)]
        public int AmenityId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal AdditionalCost { get; set; }

        [ForeignKey("RoomTypeId")]
        public virtual Data.RoomCategories RoomCategory { get; set; }

        [ForeignKey("AmenityId")]
        public virtual RoomFeatures RoomFeature { get; set; }
    }
}
