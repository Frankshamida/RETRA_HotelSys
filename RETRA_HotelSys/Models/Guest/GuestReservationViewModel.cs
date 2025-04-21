using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using RETRA_HotelSys.Data;

namespace RETRA_HotelSys.Models.ReservationManagement
{
    public class GuestReservationViewModel
    {
        public int ReservationId { get; set; }

        [Required(ErrorMessage = "Please select a guest")]
        public string GuestId { get; set; }

        [Required(ErrorMessage = "Please select a room")]
        public int RoomId { get; set; }

        [Required(ErrorMessage = "Check-in date is required")]
        [Display(Name = "Check-In Date")]
        [DataType(DataType.Date)]
        public DateTime CheckInDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Check-out date is required")]
        [Display(Name = "Check-Out Date")]
        [DataType(DataType.Date)]
        public DateTime CheckOutDate { get; set; } = DateTime.Today.AddDays(1);

        [Required(ErrorMessage = "Number of adults is required")]
        [Range(1, 10, ErrorMessage = "Number of adults must be between 1 and 10")]
        [Display(Name = "Adults")]
        public int NumberOfAdults { get; set; } = 1;

        [Range(0, 10, ErrorMessage = "Number of children must be between 0 and 10")]
        [Display(Name = "Children")]
        public int NumberOfChildren { get; set; } = 0;

        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Special Requests")]
        public string SpecialRequests { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [Display(Name = "Status")]
        public string BookingStatus { get; set; } = "Pending";

        public bool IsWalkIn { get; set; } = false;

        // For dropdowns
        [ValidateNever]
        public List<HotelGuests> AvailableGuests { get; set; }

        [ValidateNever]
        public List<HotelRooms> AvailableRooms { get; set; }
    }
}