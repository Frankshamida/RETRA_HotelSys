using Microsoft.AspNetCore.Mvc;

namespace RETRA_HotelSys.Models.Guest
{
    public class RoomType
    {
        public int RoomTypeId { get; set; }
        public string TypeName { get; set; }
        public string Description { get; set; }
        public decimal BasePrice { get; set; }
        public int MaxCapacity { get; set; }
        public string MainImagePath { get; set; }
        public int? SizeInSqFt { get; set; }
        public string BedConfiguration { get; set; }
        public List<RoomFeature> Features { get; set; } = new List<RoomFeature>();
    }
}
