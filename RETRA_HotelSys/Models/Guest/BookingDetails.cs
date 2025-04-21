using Microsoft.AspNetCore.Mvc;

namespace RETRA_HotelSys.Models.Guest
{
    public class BookingDetails
    {
        public int RoomTypeId { get; set; }
        public int RoomId { get; set; }
        public string RoomNumber { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfAdults { get; set; }
        public int NumberOfChildren { get; set; }
        public decimal BasePrice { get; set; }
        public int TotalNights { get; set; }
        public decimal TotalPrice { get; set; }
        public string BedConfiguration { get; set; }
        public string RoomImage { get; set; }
        public string SpecialRequests { get; set; }
    }

}
