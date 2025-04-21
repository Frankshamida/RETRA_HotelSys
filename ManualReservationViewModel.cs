namespace RETRA_HotelSys.Models.ReservationManagement
{
    public class ManualReservationViewModel
    {
        public string GuestFirstName { get; set; }
        public string GuestLastName { get; set; }
        public string GuestEmail { get; set; }
        public string GuestPhone { get; set; }
        public int RoomId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfAdults { get; set; }
        public int NumberOfChildren { get; set; }
        public string SpecialRequests { get; set; }
        public string BookingStatus { get; set; }
        public decimal TotalAmount { get; set; }

        public List<AvailableRoom> AvailableRooms { get; set; }
    }

    public class AvailableRoom
    {
        public int RoomId { get; set; }
        public string RoomNumber { get; set; }
        public string RoomType { get; set; }
        public decimal BasePrice { get; set; }
    }
}
