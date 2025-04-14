using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RETRA_HotelSys.Data
{
    public class StaffRoles
    {
        [Key]
        public int RoleId { get; set; }

        [Required]
        [StringLength(50)]
        public string RoleName { get; set; }

        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ModifiedDate { get; set; }

        // Navigation property
        public virtual ICollection<StaffMembers> StaffMembers { get; set; } = new List<StaffMembers>();
    }
}