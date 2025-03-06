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
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;

        public UserController(ApplicationDbContext db)
        {
            _db = db;
        }

        [Authorize]
        public IActionResult Index()
        {
            var Users = _db.Users.ToList();
            var model = new HomeViewModel
            {
                AvailableUsers = Users
            };
            return View(model);
        }

        [HttpGet]
        [Authorize]
        public IActionResult Delete(int id)
        {
            var user = _db.Users.FirstOrDefault(v => v.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = _db.Users.FirstOrDefault(v => v.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
            TempData["Success"] = "User deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
