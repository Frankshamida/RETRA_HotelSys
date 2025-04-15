using Microsoft.AspNetCore.Mvc;
using RETRA_HotelSys.Data;

namespace RETRA_HotelSys.Models.RoomManagement
{
    public class RoomDetailsViewModel
    {
        public HotelRooms Room { get; set; }
        public ReservationViewModel CurrentReservation { get; set; }
        public List<ReservationViewModel> UpcomingReservations { get; set; }
        public List<MaintenanceViewModel> MaintenanceHistory { get; set; }
        public List<RoomFeatures> RoomFeatures { get; set; }
    }
}
