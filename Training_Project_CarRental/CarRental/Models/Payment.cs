using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRental.Models
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string CardholderName { get; set; }

        [Required]
        [StringLength(16)]
        public string CardNumber { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; }

        [Required]
        public int CVV { get; set; }

        [Required]
        public int user_Id { get; set; }

        [ForeignKey(nameof(user_Id))]
        public User User { get; set; }
        public ICollection<Rental> Rentals { get; set; }
    }
}
