using Microsoft.AspNetCore.Mvc;
using CarRental.Models.ViewModels;
using CarRental.Data;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Security.Claims;
using CarRental.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;

namespace CarRental.Controllers
{
    [Authorize]
    public class RentalController : Controller
    {
        private readonly ApplicationDbContext _db;

        public RentalController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            var userEmail = User.Identity.Name;
            var userId = _db.Users
                             .Where(u => u.email == userEmail)
                             .Select(u => u.Id)
                             .FirstOrDefault();

            var rentals = _db.Rentals
                              .Where(r => r.user_id == userId)
                              .Select(r => new RentalViewModel
                              {
                                  RentalId = r.Id,
                                  VehicleBrand = r.Vehicle.brand,
                                  VehicleModel = r.Vehicle.model,
                                  RentalStartDate = r.start_date,
                                  RentalEndDate = r.end_date,
                                  TotalCost = r.total_cost,
                                  PaymentMethod = r.Payment != null ? r.Payment.CardNumber : "N/A"
                              })
                              .ToList();

            var model = new RentalListViewModel
            {
                Rentals = rentals
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult CancelRental(int rentalId)
        {
            var rental = _db.Rentals.FirstOrDefault(r => r.Id == rentalId);
            if (rental != null)
            {
                try
                {
                    _db.Rentals.Remove(rental);
                    _db.SaveChanges();
                }
                catch (DbUpdateException dbEx)
                {
                    Console.WriteLine($"DbUpdateException: {dbEx.Message}");
                    if (dbEx.InnerException != null)
                    {
                        Console.WriteLine($"Inner Exception: {dbEx.InnerException.Message}");
                    }
                    ModelState.AddModelError("", "An error occurred while canceling the rental.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception: {ex.Message}");
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                    }
                    ModelState.AddModelError("", "An unexpected error occurred.");
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Book(int vehicleId, DateTime pickupDate, DateTime dropoffDate, string cardNumber, string expiryDate, int CVV)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            var email = User.Identity.Name;
            var user = await _db.Users.SingleOrDefaultAsync(u => u.email == email);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                return View();
            }

            var vehicle = await _db.Vehicles.FindAsync(vehicleId);
            if (vehicle == null)
            {
                ModelState.AddModelError("", "Vehicle not found.");
                return View();
            }

            if (pickupDate >= dropoffDate)
            {
                ModelState.AddModelError("", "Drop-off date must be after pickup date.");
                return View();
            }

            var totalCost = (dropoffDate - pickupDate).TotalDays * (vehicle.price_per_day ?? 0);

            var payment = new Payment
            {
                CardholderName = user.first_name + " " + user.last_name,
                CardNumber = cardNumber.Replace("-", "").Trim(),
                ExpiryDate = DateTime.ParseExact(expiryDate, "MM/yy", null),
                CVV = CVV,
                user_Id = user.Id
            };

            try
            {
                _db.Payments.Add(payment);
                await _db.SaveChangesAsync();

                var rental = new Rental
                {
                    start_date = pickupDate,
                    end_date = dropoffDate,
                    total_cost = totalCost,
                    user_id = user.Id,
                    vehicle_id = vehicleId,
                    payment_id = payment.Id
                };

                _db.Rentals.Add(rental);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
                TempData["Success"] = "Rental Done successfully";
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"DbUpdateException: {dbEx.Message}");
                if (dbEx.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {dbEx.InnerException.Message}");
                    Console.WriteLine($"Inner Exception Stack Trace: {dbEx.InnerException.StackTrace}");
                }
                ModelState.AddModelError("", "An error occurred while saving the payment.");
                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                ModelState.AddModelError("", "An unexpected error occurred.");
                return View();
            }
        }
    }
}
