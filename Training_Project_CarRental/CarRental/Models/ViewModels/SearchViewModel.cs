namespace CarRental.Models.ViewModels
{
    public class SearchViewModel
    {
        public List<Vehicle> AvailableCars { get; set; } = new List<Vehicle>();
        public string Location { get; set; }
        public DateTime? PickupDate { get; set; }
        public DateTime? DropoffDate { get; set; }
    }
}
