using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CarRental.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string first_name { get; set; }

        [Required]
        public string last_name { get; set; }

        public string gender { get; set; }

        [StringLength(100)]
        public string address { get; set; }

        [Required]
        public int phone_no { get; set; }

        [Required]
        [EmailAddress]
        public string email { get; set; }

        [Required]
        [StringLength(50)]
        public string password { get; set; }

        public ICollection<Payment> Payments { get; set; }
        public ICollection<Rental> Rentals { get; set; }
    }
}
