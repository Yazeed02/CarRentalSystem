using CarRental.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarRental.Models.ViewModels
{
    public class VehicleViewModel
    {
        public Vehicle Vehicle { get; set; }
        public DateTime? pickup { get; set; }
        public DateTime? dropoff { get; set; }
    }
}
