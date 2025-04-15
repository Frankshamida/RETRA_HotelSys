using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RETRA_HotelSys.Models.RoomManagement
{
    public class CreateRoomViewModel
    {
        public int RoomTypeId { get; set; }
        public string CategoryName { get; set; }

        [Required(ErrorMessage = "Room number is required")]
        [StringLength(20, ErrorMessage = "Room number cannot exceed 20 characters")]
        [Remote(action: "VerifyRoomNumber", controller: "Admin", AdditionalFields = "RoomTypeId",
                ErrorMessage = "Room number already exists in this category")]
        public string RoomNumber { get; set; }

        [Required(ErrorMessage = "Floor number is required")]
        [Range(1, 20, ErrorMessage = "Floor must be between 1 and 20")]
        public int FloorNumber { get; set; }

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        public string Notes { get; set; }

        public List<SelectListItem> FloorOptions { get; set; } = new List<SelectListItem>();
    }
}
