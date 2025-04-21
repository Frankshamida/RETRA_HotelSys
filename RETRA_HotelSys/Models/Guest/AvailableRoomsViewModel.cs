using Microsoft.AspNetCore.Mvc;

namespace RETRA_HotelSys.Models.Guest
{
    public class AvailableRoomsViewModel
    {
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfAdults { get; set; }
        public int NumberOfChildren { get; set; }
        public int TotalGuests => NumberOfAdults + NumberOfChildren;
        public List<RoomType> ExactMatchRooms { get; set; } = new List<RoomType>();
        public List<RoomType> NearMatchRooms { get; set; } = new List<RoomType>();
        public List<Data.RoomCategories> AvailableRoomCategories { get; internal set; }
        internal RoomSearchViewModel SearchCriteria { get; set; }
    }
}
