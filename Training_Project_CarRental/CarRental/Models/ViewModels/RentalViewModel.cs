namespace CarRental.Models.ViewModels
{
    public class RentalViewModel
    {
        public int RentalId { get; set; }
        public string VehicleBrand { get; set; }
        public string VehicleModel { get; set; }
        public DateTime RentalStartDate { get; set; }
        public DateTime RentalEndDate { get; set; }
        public double TotalCost { get; set; }
        public string PaymentMethod { get; set; }
    }

    public class RentalListViewModel
    {
        public List<RentalViewModel> Rentals { get; set; }
    }
}
