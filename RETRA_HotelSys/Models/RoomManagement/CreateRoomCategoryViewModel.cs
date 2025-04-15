using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using RETRA_HotelSys.Data;

namespace RETRA_HotelSys.Models.RoomManagement
{
    public class CreateRoomCategoryViewModel
    {
        [Required(ErrorMessage = "Category name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string TypeName { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Base price is required")]
        [Range(0, 10000, ErrorMessage = "Price must be between 0 and 10,000")]
        public decimal BasePrice { get; set; }

        [Required(ErrorMessage = "Maximum capacity is required")]
        [Range(1, 10, ErrorMessage = "Capacity must be between 1 and 10")]
        public int MaxCapacity { get; set; }

        [Range(0, 5000, ErrorMessage = "Size must be between 0 and 5,000")]
        public int? SizeInSqFt { get; set; }

        [StringLength(100, ErrorMessage = "Bed configuration cannot exceed 100 characters")]
        public string BedConfiguration { get; set; }

        [Display(Name = "Main Image")]
        [StringLength(255, ErrorMessage = "Image path cannot exceed 255 characters")]
        public string MainImagePath { get; set; }

        public List<int> SelectedFeatureIds { get; set; } = new List<int>();
        public List<RoomFeatures> AllFeatures { get; set; } = new List<RoomFeatures>();
    }
}
