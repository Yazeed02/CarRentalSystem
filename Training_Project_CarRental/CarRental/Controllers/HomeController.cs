using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CarRental.Data;
using CarRental.Models;
using CarRental.Models.ViewModels;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;

namespace CarRental.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }
        public IActionResult Index()
        {
            var vehicles = _db.Vehicles.OrderBy(v => v.brand).ToList();
            var rentals = _db.Rentals.ToList();

            var vehicleStatuses = vehicles.Select(vehicle => new VehicleStatusViewModel
            {
                Vehicle = vehicle,
                IsRented = rentals.Any(r => r.vehicle_id == vehicle.Id &&
                                             r.end_date >= DateTime.Now &&
                                             r.start_date <= DateTime.Now)
            }).ToList();

            var model = new HomeViewModel
            {
                VehicleStatuses = vehicleStatuses
            };

            return View(model);
        }
        public IActionResult LoadMoreCars(int start, int count)
        {
            var vehicles = _db.Vehicles
                .OrderBy(v => v.brand)
                .Skip(start)
                .Take(count)
                .ToList();

            var rentals = _db.Rentals.ToList();

            var vehicleStatuses = vehicles.Select(vehicle => new VehicleStatusViewModel
            {
                Vehicle = vehicle,
                IsRented = rentals.Any(r => r.vehicle_id == vehicle.Id &&
                                             r.end_date >= DateTime.Now &&
                                             r.start_date <= DateTime.Now)
            }).ToList();

            return PartialView("_CarListPartial", vehicleStatuses);
        }
        public IActionResult Search(string location, DateTime? pickupDate, DateTime? dropoffDate)
        {
            var model = new SearchViewModel
            {
                Location = location,
                PickupDate = pickupDate,
                DropoffDate = dropoffDate
            };

            if (string.IsNullOrEmpty(location) || !pickupDate.HasValue || !dropoffDate.HasValue)
            {
                ModelState.AddModelError("", "Please provide all search criteria.");
                return View(model);
            }

            if (pickupDate >= dropoffDate)
            {
                ModelState.AddModelError("", "Drop-off date must be after pickup date.");
                return View(model);
            }

            var availableCars = _db.Vehicles
                .Where(v => v.location.ToLower().Contains(location.ToLower()))
                .ToList();

            model.AvailableCars = availableCars;

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            var model = new ContactViewModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult SendContact(ContactViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Handle form submission
                // You can use a service to send an email, save to database, etc.
            }
            return RedirectToAction("index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Details(int id, DateTime? pickupDate, DateTime? dropoffDate)
        {
            var vehicle = _db.Vehicles.Find(id);

            if (vehicle == null)
            {
                return NotFound();
            }

            var model = new VehicleDetailsViewModel
            {
                Vehicle = vehicle,
                PickupDate = pickupDate,
                DropoffDate = dropoffDate
            };

            return View(model);
        }
    }
}
