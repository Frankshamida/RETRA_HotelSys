using Microsoft.AspNetCore.Mvc;

// Models/RoomFeature.cs
namespace RETRA_HotelSys.Models
{
    public class RoomFeature
    {
        public int RoomFeatureId { get; set; }
        public string FeatureName { get; set; }
        public string IconClass { get; set; }
    }
}

