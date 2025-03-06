using System;
using System.ComponentModel.DataAnnotations;

namespace CarRental.Models.ViewModels
{
    public class BookingViewModel
    {
        public int VehicleId { get; set; }
        [Required]
        public DateTime PickupDate { get; set; }
        [Required]
        public DateTime DropoffDate { get; set; }
        [Required]
        public string CardholderName { get; set; }
        [Required]
        public string CardNumber { get; set; }
        [Required]
        public string ExpiryDate { get; set; }
        [Required]
        public string Cvv { get; set; }
    }
}