
using RETRA_HotelSys.Data;

namespace RETRA_HotelSys.Controllers
{
    internal class ApplicationUser
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public object FirstName { get; set; }
        public object LastName { get; set; }
        public object PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string Id { get; internal set; }

        public static implicit operator ApplicationUser?(HotelGuests? v)
        {
            throw new NotImplementedException();
        }
    }
}