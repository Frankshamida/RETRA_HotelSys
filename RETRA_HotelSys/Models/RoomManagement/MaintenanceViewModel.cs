using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;

namespace RETRA_HotelSys.Models.RoomManagement
{
    public class MaintenanceViewModel
    {
        public DateTime ScheduledDate { get; set; }
        public string Type { get; set; } // Or rename this to MaintenanceType if preferred
        public string Status { get; set; }
    }
}
