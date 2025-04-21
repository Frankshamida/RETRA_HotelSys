namespace RETRA_HotelSys.Models.Guest
{
    internal class GuestInfoModel
    {
        public string GuestEmail { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string SpecialRequests { get; set; }
        public bool CreateAccount { get; set; }
    }
}