using Microsoft.AspNetCore.Mvc;
using RETRA_HotelSys.Data;

namespace RETRA_HotelSys.Models.RoomManagement
{
    public class RoomManagementViewModel
    {
        public List<RoomCategories> RoomCategories { get; set; }
        public List<RoomFeatures> AllFeatures { get; set; }
        public RoomStatusSummary RoomStatusSummary { get; set; }
    }
}
