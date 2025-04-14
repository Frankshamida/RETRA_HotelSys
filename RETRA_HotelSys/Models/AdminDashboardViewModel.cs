using System.Collections.Generic;
using RETRA_HotelSys.Data;

namespace RETRA_HotelSys.Models
{
    public class AdminDashboardViewModel
    {
        public int TotalRooms { get; set; }
        public int AvailableRooms { get; set; }
        public int TotalStaff { get; set; }
        public int ActiveReservations { get; set; }
        public List<GuestReservations> RecentReservations { get; set; }
        public List<RoomMaintenance> MaintenanceRequests { get; set; }
        public object RevenueThisMonth { get; internal set; }
        public string AdminName { get; internal set; }
        public string? AdminEmail { get; internal set; }
        public DateTime? LastLogin { get; internal set; }
        public int OccupiedRooms { get; internal set; }
    }
}