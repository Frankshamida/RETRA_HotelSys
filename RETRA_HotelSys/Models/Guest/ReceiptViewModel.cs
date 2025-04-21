using Microsoft.AspNetCore.Mvc;

namespace RETRA_HotelSys.Models.Guest
{
    public class ReceiptViewModel
    {
        public int PaymentId { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; }
        public string TransactionId { get; set; }
        public decimal AmountPaid { get; set; }

        public int ReservationId { get; set; }
        public string GuestName { get; set; }
        public string RoomType { get; set; }
        public string RoomNumber { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfNights { get; set; }
        public decimal BasePrice { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
