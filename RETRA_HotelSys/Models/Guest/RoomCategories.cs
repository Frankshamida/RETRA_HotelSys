using System.Collections.Generic;
using RETRA_HotelSys.Data;

namespace RETRA_HotelSys.Models.Guest
{
    public class RoomCategories
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public decimal BasePrice { get; set; }
        public int MaxCapacity { get; set; }
        public string BedType { get; set; }
        public int RoomSize { get; set; }
        public string ImageUrl { get; set; }
        public List<RoomTypeFeature> RoomTypeFeatures { get; set; }
    }

    public class RoomTypeFeature
    {
        public RoomFeature RoomFeature { get; set; }
    }

    public class RoomFeature
    {
        public string FeatureName { get; set; }
        public string IconClass { get; internal set; }
    }
}