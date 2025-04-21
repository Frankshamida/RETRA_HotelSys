using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RETRA_HotelSys.Data;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RETRA_HotelSys.Models.RoomManagement
{
   public class CreateRoomCategoryViewModel
{
    [Display(Name = "Category Name")]
    [Required(ErrorMessage = "Category name is required")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    public string TypeName { get; set; }

    [Display(Name = "Description")]
    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string Description { get; set; }

    [Display(Name = "Room Type")]
    public string SelectedRoomType { get; set; }

    public List<SelectListItem> RoomTypes { get; set; } = new List<SelectListItem>
    {
        new SelectListItem { Value = "Standard", Text = "Standard" },
        new SelectListItem { Value = "Deluxe", Text = "Deluxe" },
        new SelectListItem { Value = "Suite", Text = "Suite" }
    };

    [Display(Name = "Base Price")]
    [Required(ErrorMessage = "Base price is required")]
    [Range(0, 10000, ErrorMessage = "Price must be between 0 and 10,000")]
    public decimal BasePrice { get; set; }

    [Display(Name = "Max Capacity")]
    [Required(ErrorMessage = "Maximum capacity is required")]
    [Range(1, 10, ErrorMessage = "Capacity must be between 1 and 10")]
    public int MaxCapacity { get; set; }

    [Display(Name = "Size (sq ft)")]
    [Range(0, 5000, ErrorMessage = "Size must be between 0 and 5,000")]
    public int? SizeInSqFt { get; set; }

    [Display(Name = "Bed Configuration")]
    [StringLength(100, ErrorMessage = "Bed configuration cannot exceed 100 characters")]
    public string BedConfiguration { get; set; }

    [Display(Name = "Upload Image")]
    [DataType(DataType.Upload)]
    public IFormFile ImageFile { get; set; }

    // Change this to just store the path/URL
    [Display(Name = "Image URL")]
    [StringLength(500, ErrorMessage = "Image URL cannot exceed 500 characters")]
    public string ImageUrl { get; set; }

    // For feature selection
    public List<RoomFeatureSelection> AllFeatures { get; set; } = new List<RoomFeatureSelection>();
    public List<int> SelectedFeatureIds { get; set; } = new List<int>();
}

public class RoomFeatureSelection
{
    public int AmenityId { get; set; }
    public string Name { get; set; }
    public string IconClass { get; set; }
    public bool IsSelected { get; set; }
}
}