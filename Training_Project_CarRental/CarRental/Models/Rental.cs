using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRental.Models
{
    public class Rental
    {
        [Key]
        public int Id { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public double total_cost { get; set; }

        public int user_id { get; set; }
        public int vehicle_id { get; set; }
        public int payment_id { get; set; }

        [ForeignKey("user_id")]
        public User User { get; set; }

        [ForeignKey("vehicle_id")]
        public Vehicle Vehicle { get; set; }

        [ForeignKey("payment_id")]
        public Payment Payment { get; set; }
    }
}
