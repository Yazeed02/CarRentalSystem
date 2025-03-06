namespace CarRental.Models.ViewModels
{
    public class VehicleDetailsViewModel
    {
        public Vehicle Vehicle { get; set; }
        public DateTime? PickupDate { get; set; }
        public DateTime? DropoffDate { get; set; }
    }
}