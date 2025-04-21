namespace RETRA_HotelSys.Models.Guest
{
    internal class RoomAvailabilityViewModel
    {
        public int RoomTypeId { get; set; }
        public string TypeName { get; set; }
        public string Description { get; set; }
        public decimal BasePrice { get; set; }
        public int MaxCapacity { get; set; }
        public int AvailableCount { get; set; }
        public string ImagePath { get; set; }
        public string BedConfiguration { get; set; }
        public int? SizeInSqFt { get; internal set; }
    }
}