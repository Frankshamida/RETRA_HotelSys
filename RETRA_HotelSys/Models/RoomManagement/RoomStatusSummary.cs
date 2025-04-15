using Microsoft.AspNetCore.Mvc;

namespace RETRA_HotelSys.Models.RoomManagement
{
    public class RoomStatusSummary
    {
        public int TotalRooms { get; set; }
        public int AvailableRooms { get; set; }
        public int OccupiedRooms { get; set; }
        public int MaintenanceRooms { get; set; }
    }
}
