using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RETRA_HotelSys.Models.RoomManagement
{
    public class EditRoomViewModel
    {
        public int RoomId { get; set; }
        public int RoomTypeId { get; set; }
        public string CategoryName { get; set; }

        [Required]
        [StringLength(20)]
        public string RoomNumber { get; set; }

        [Required]
        [Range(1, 20)]
        public int FloorNumber { get; set; }

        public bool IsAvailable { get; set; }
        public bool IsClean { get; set; }

        [StringLength(500)]
        public string Notes { get; set; }

        public List<SelectListItem> FloorOptions { get; set; }
    }
}
