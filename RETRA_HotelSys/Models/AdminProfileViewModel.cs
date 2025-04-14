using System.Collections.Generic;
using RETRA_HotelSys.Data;

namespace RETRA_HotelSys.Models
{
    public class AdminProfileViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public object LastAccessDate { get; set; }
    }
}