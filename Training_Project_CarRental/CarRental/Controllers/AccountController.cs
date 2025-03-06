using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarRental.Data;
using CarRental.Models;
using CarRental.Models.ViewModels;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

public class AccountController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger<AccountController> _logger;

    public AccountController(ApplicationDbContext db, ILogger<AccountController> logger)
    {
        _db = db;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var admin = await _db.Admins
                .SingleOrDefaultAsync(a => a.email == model.Email && a.password == model.Password);
            if (admin != null)
            {
                await SignInAsync(admin.email, "Admin");
                return RedirectToAction("Index", "Home");
            }

            var user = await _db.Users
                .SingleOrDefaultAsync(u => u.email == model.Email && u.password == model.Password);
            if (user != null)
            {
                await SignInAsync(user.email, "User");
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        }
        return View(model);
    }

    private async Task SignInAsync(string email, string role)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, email),
            new Claim(ClaimTypes.Role, role)
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity));
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> EditProfile()
    {
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Login", "Account");
        }

        var email = User.Identity.Name;
        var user = await _db.Users.SingleOrDefaultAsync(u => u.email == email);

        if (user == null)
        {
            return NotFound();
        }

        var model = new EditProfileViewModel
        {
            FirstName = user.first_name,
            LastName = user.last_name,
            Email = user.email,
            Gender = user.gender,
            Address = user.address,
            PhoneNo = user.phone_no
        };

        return View(model);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> EditProfile(EditProfileViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Login", "Account");
        }

        var email = User.Identity.Name;
        var user = await _db.Users.SingleOrDefaultAsync(u => u.email == email);

        if (user == null)
        {
            return NotFound();
        }

        user.first_name = model.FirstName;
        user.last_name = model.LastName;
        user.gender = model.Gender;
        user.address = model.Address;
        user.phone_no = model.PhoneNo;

        await _db.SaveChangesAsync();

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            bool emailExists = await _db.Users.AnyAsync(u => u.email == model.Email) ||
                               await _db.Admins.AnyAsync(a => a.email == model.Email);

            if (emailExists)
            {
                ModelState.AddModelError(string.Empty, "Email is already in use.");
                return View(model);
            }

            var user = new User
            {
                first_name = model.FirstName,
                last_name = model.LastName,
                email = model.Email,
                password = model.Password,
                gender = model.Gender,
                address = model.Address,
                phone_no = model.PhoneNo
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            await SignInAsync(user.email, "User");
            return RedirectToAction("Index", "Home");
        }
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> EditProfileAdmin()
    {
        var email = User.Identity.Name;
        var admin = await _db.Admins.SingleOrDefaultAsync(u => u.email == email);

        if (admin == null)
        {
            return NotFound();
        }

        var model = new EditProfileAdminViewModel
        {
            FirstName = admin.first_name,
            LastName = admin.last_name,
            Email = admin.email,
            Password = admin.password
        };

        return View(model);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> EditProfileAdmin(EditProfileAdminViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var email = User.Identity.Name;
        var admin = await _db.Admins.SingleOrDefaultAsync(u => u.email == email);

        if (admin == null)
        {
            return NotFound();
        }

        admin.first_name = model.FirstName;
        admin.last_name = model.LastName;
        admin.email = model.Email;
        admin.password = model.Password;

        await _db.SaveChangesAsync();

        return RedirectToAction("Index", "Home");
    }

    public IActionResult AccessDenied()
    {
        return View();
    }
}
