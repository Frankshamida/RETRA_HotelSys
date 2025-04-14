using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace RETRA_HotelSys.Data
{
    public class HotelGuests : IdentityUser
    {
        [PersonalData]
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [PersonalData]
        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [PersonalData]
        [StringLength(200)]
        public string? Address { get; set; }

        [PersonalData]
        [StringLength(100)]
        public string? City { get; set; }

        [PersonalData]
        [StringLength(100)]
        public string? StateProvince { get; set; }

        [PersonalData]
        [StringLength(100)]
        public string? Country { get; set; }

        [PersonalData]
        [StringLength(20)]
        public string? PostalCode { get; set; }

        [PersonalData]
        [StringLength(50)]
        public string? IDType { get; set; }

        [PersonalData]
        [StringLength(50)]
        public string? IDNumber { get; set; }

        [PersonalData]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        public bool IsActive { get; set; } = true;

        [Required]
        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        public DateTime? LastUpdated { get; set; }

        [StringLength(255)]
        public string? ProfilePicture { get; set; }

        public DateTime? LastLoginDate { get; set; }

        // Navigation property for reservations
        public virtual ICollection<GuestReservations> GuestReservations { get; set; } = new List<GuestReservations>();

        [NotMapped]
        public string Initials => $"{FirstName?[0]}{LastName?[0]}".ToUpper();

        [NotMapped]
        public string DisplayName => $"{FirstName} {LastName}";

        [Column(TypeName = "datetime2")]
        public DateTime? LastAccessDate { get; set; }
    }
}