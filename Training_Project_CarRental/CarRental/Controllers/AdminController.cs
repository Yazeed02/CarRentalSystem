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
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AdminController(ApplicationDbContext db)
        {
            _db = db;
        }

        [Authorize]
        public IActionResult Index()
        {
            var Admins = _db.Admins.ToList();
            var model = new HomeViewModel
            {
                AvailableAdmins = Admins
            };
            return View(model);
        }
        [HttpGet]
        [Authorize]
        public IActionResult Create(int? id)
        {
            var viewModel = new AdminViewModel
            {
                admin = id == null || id == 0 ? new Admin() : _db.Admins.FirstOrDefault(v => v.Id == id)
            };

            if (viewModel.admin == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Create(AdminViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                _db.Admins.Add(viewModel.admin);
                TempData["Success"] = "Admin added successfully";
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(viewModel);
        }

        [HttpGet]
        [Authorize]
        public IActionResult Delete(int id)
        {
            var admin = _db.Admins.FirstOrDefault(v => v.Id == id);
            if (admin == null)
            {
                return NotFound();
            }

            return View(admin);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var admin = _db.Admins.FirstOrDefault(v => v.Id == id);
            if (admin == null)
            {
                return NotFound();
            }

            _db.Admins.Remove(admin);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Admin deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
