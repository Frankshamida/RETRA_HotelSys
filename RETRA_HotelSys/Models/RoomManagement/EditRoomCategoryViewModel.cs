using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using RETRA_HotelSys.Data;

namespace RETRA_HotelSys.Models.RoomManagement
{
    public class EditRoomCategoryViewModel
    {
        public int RoomTypeId { get; set; }

        [Required]
        [StringLength(100)]
        public string TypeName { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        [Range(0, 10000)]
        public decimal BasePrice { get; set; }

        [Required]
        [Range(1, 10)]
        public int MaxCapacity { get; set; }

        [Range(0, 5000)]
        public int? SizeInSqFt { get; set; }

        [StringLength(100)]
        public string BedConfiguration { get; set; }

        [Display(Name = "Main Image")]
        public string MainImagePath { get; set; }

        public List<int> SelectedFeatureIds { get; set; }
        public List<RoomFeatures> AllFeatures { get; set; }
    }
}
