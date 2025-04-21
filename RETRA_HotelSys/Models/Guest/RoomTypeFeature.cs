using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using RETRA_HotelSys.Data;

// Models/RoomTypeFeature.cs
namespace RETRA_HotelSys.Models
{
    public class RoomTypeFeature
    {
        public int RoomTypeFeatureId { get; set; }
        public int RoomCategoryId { get; set; }
        public int RoomFeatureId { get; set; }

        // Navigation properties
        public RoomCategories RoomCategory { get; set; }
        public RoomFeature RoomFeature { get; set; }
    }
}