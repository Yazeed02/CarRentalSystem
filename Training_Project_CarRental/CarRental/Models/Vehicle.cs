using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRental.Models
{
    public class Vehicle
    {
        [Key]
        public int Id { get; set; }
        [DisplayName("Vehicle model")]
        public string model { get; set; }
		[Range(1950,2025)]
        [DisplayName("Year")]
        public int? year { get; set; }
		[DisplayName("Vehicle type")]
		public string type { get; set; }
        [DisplayName("Vehicle Capacity")]
        public string capacity { get; set; }
        [Range(1,1000)]
		[DisplayName("Price per day")]
		public int? price_per_day{ get; set; }
		[DisplayName("Location")]
		public string location { get; set; }
		[DisplayName("Color")]
		public string color { get; set; }
		[DisplayName("Vehicle Brand")]
		public string brand { get; set; }
        public string? imageUrl { get; set; }
        [ValidateNever]
        public ICollection<Rental> Rentals { get; set; }
    }
}
