using System.Collections.Generic;

namespace CarRental.Models.ViewModels
{
    public class HomeViewModel
    {
        public IEnumerable<Vehicle> AvailableCars { get; set; }
        public IEnumerable<User> AvailableUsers { get; set; }
        public IEnumerable<Admin> AvailableAdmins { get; set; }
        public string Location { get; set; }
        public DateTime? PickupDate { get; set; }
        public DateTime? DropoffDate { get; set; }
        public IEnumerable<VehicleStatusViewModel> VehicleStatuses { get; set; }
    }
}
