using Microsoft.AspNetCore.Mvc;
using RETRA_HotelSys.Data;

namespace RETRA_HotelSys.Models.RoomManagement
{
    public class RoomsByCategoryViewModel
    {
        public RoomCategories Category { get; set; }
        public List<HotelRooms> Rooms { get; set; }
    }
}
