// StaffManagementViewModel.cs
using RETRA_HotelSys.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RETRA_HotelSys.Models.StaffManagement
{
    public class StaffManagementViewModel
    {
        public List<StaffMembers> StaffMembers { get; set; } = new List<StaffMembers>();
        public List<StaffRoles> AvailableRoles { get; set; } = new List<StaffRoles>();
    }
}