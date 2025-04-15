using Microsoft.AspNetCore.Mvc;

namespace RETRA_HotelSys.Models.RoomManagement
{
    public class ReservationViewModel
    {
        public string GuestName { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public string BookingStatus { get; set; }
    }

}
