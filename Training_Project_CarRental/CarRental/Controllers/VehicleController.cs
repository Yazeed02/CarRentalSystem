using CarRental.Data;
using CarRental.Models;
using CarRental.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CarRental.Controllers
{
    [Authorize(Roles = "Admin")]
    public class VehicleController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public VehicleController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }

        [Authorize]
        public IActionResult Index()
        {
            var vehicles = _db.Vehicles.ToList();
            var model = new HomeViewModel
            {
                AvailableCars = vehicles
            };
            return View(model);
        }

        [HttpGet]
        [Authorize]
        public IActionResult Upsert(int? id)
        {
            var viewModel = new VehicleViewModel
            {
                Vehicle = id == null || id == 0 ? new Vehicle() : _db.Vehicles.FirstOrDefault(v => v.Id == id)
            };

            if (viewModel.Vehicle == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Upsert(VehicleViewModel viewModel, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string vehiclePath = Path.Combine(wwwRootPath, "images", "vehicles");

                Vehicle vehicle;
                if (viewModel.Vehicle.Id == 0)
                {
                    vehicle = new Vehicle
                    {
                        brand = viewModel.Vehicle.brand,
                        model = viewModel.Vehicle.model,
                        year = viewModel.Vehicle.year,
                        type = viewModel.Vehicle.type,
                        capacity = viewModel.Vehicle.capacity,
                        price_per_day = viewModel.Vehicle.price_per_day,
                        location = viewModel.Vehicle.location,
                        color = viewModel.Vehicle.color,
                        imageUrl = file != null && file.Length > 0
                            ? await SaveImageAsync(file, vehiclePath)
                            : "images/vehicles/default.jpg"
                    };
                    _db.Vehicles.Add(vehicle);
                    TempData["Success"] = "Vehicle added successfully";
                }
                else
                {
                    vehicle = _db.Vehicles.FirstOrDefault(v => v.Id == viewModel.Vehicle.Id);
                    if (vehicle == null)
                    {
                        ModelState.AddModelError("", "Vehicle not found.");
                        return View(viewModel);
                    }

                    vehicle.brand = viewModel.Vehicle.brand;
                    vehicle.model = viewModel.Vehicle.model;
                    vehicle.year = viewModel.Vehicle.year;
                    vehicle.type = viewModel.Vehicle.type;
                    vehicle.capacity = viewModel.Vehicle.capacity;
                    vehicle.price_per_day = viewModel.Vehicle.price_per_day;
                    vehicle.location = viewModel.Vehicle.location;
                    vehicle.color = viewModel.Vehicle.color;

                    if (file != null && file.Length > 0)
                    {
                        if (!string.IsNullOrEmpty(vehicle.imageUrl))
                        {
                            var oldImagePath = Path.Combine(wwwRootPath, vehicle.imageUrl);
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        vehicle.imageUrl = await SaveImageAsync(file, vehiclePath);
                    }

                    _db.Vehicles.Update(vehicle);
                    TempData["Success"] = "Vehicle updated successfully";
                }

                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(viewModel);
        }
        private async Task<string> SaveImageAsync(IFormFile file, string vehiclePath)
        {
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            if (!Directory.Exists(vehiclePath))
            {
                Directory.CreateDirectory(vehiclePath);
            }
            string filePath = Path.Combine(vehiclePath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return "images/vehicles/" + fileName;
        }


        [HttpGet]
        [Authorize]
        public IActionResult Delete(int id)
        {
            var vehicle = _db.Vehicles.FirstOrDefault(v => v.Id == id);
            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vehicle = _db.Vehicles.FirstOrDefault(v => v.Id == id);
            if (vehicle == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(vehicle.imageUrl))
            {
                var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, vehicle.imageUrl);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _db.Vehicles.Remove(vehicle);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Vehicle deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
