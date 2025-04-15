using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace RETRA_HotelSys.Data
{
    public class RoomTypeImages
    {
        [Key]
        public int ImageId { get; set; }

        [ForeignKey("RoomCategory")]
        public int RoomTypeId { get; set; }

        [Required]
        [StringLength(255)]
        public string ImagePath { get; set; }

        public int DisplayOrder { get; set; }
        public bool IsPrimary { get; set; }

        public virtual RoomCategories RoomCategory { get; set; }
    }
}
