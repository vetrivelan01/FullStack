using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using FullStack.Models;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using FullStack.Helpers;
using System;

namespace FullStack.Controllers
{
    public class LoginController : Controller
    {
        private readonly FullStackDbContext _context;

        public LoginController(FullStackDbContext context)
        {
            _context = context;
        }

        // GET: Login
        public IActionResult Index()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginDetails login)
        {
            ModelState.Remove("LoginType"); // LoginType is not submitted in the form
            if (ModelState.IsValid)
            {
                var user = await _context.Logins.FirstOrDefaultAsync(l => l.LoginID == login.LoginID);
                if (user != null && PasswordHelper.VerifyPassword(login.Password, user.Password))
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.LoginID),
                        new Claim(ClaimTypes.Role, user.LoginType ?? "User"), // Default to User if null
                        new Claim("CitizenID", user.LoginID) // Assume LoginID is the CitizenID for users
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity));

                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Invalid Login Attempt");
            }
            return View(login);
        }

        // GET: Register
        [AllowAnonymous]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            var today = DateTime.Today;
            var age = today.Year - model.DateOfBirth.Year;
            if (model.DateOfBirth.Date > today.AddYears(-age))
            {
                age--;
            }

            if (age < 18)
            {
                ModelState.AddModelError("DateOfBirth", "You must be at least 18 years old to register.");
            }

            if (ModelState.IsValid)
            {
                // Check if user already exists
                if (await _context.Logins.AnyAsync(l => l.LoginID == model.LoginID))
                {
                    ModelState.AddModelError("LoginID", "This Login ID is already taken.");
                    return View(model);
                }

                // Create Login details
                var newLogin = new LoginDetails
                {
                    LoginID = model.LoginID,
                    Password = PasswordHelper.HashPassword(model.Password),
                    LoginType = "User" // Default newly registered users to "User" role
                };
                _context.Logins.Add(newLogin);

                // Create associated Citizen profile
                var newCitizen = new Citizen
                {
                    CitizenID = model.LoginID, // Tying CitizenID to LoginID exactly
                    CitizenName = model.CitizenName,
                    Gender = model.Gender,
                    DateOfBirth = model.DateOfBirth,
                    Nationality = model.Nationality,
                    PhoneNumber = model.PhoneNumber,
                    Address = "Pending Address Update", // Fixing NULL constraint on Registration
                    Language = "English"
                };
                _context.Citizens.Add(newCitizen);

                await _context.SaveChangesAsync();

                // Automatically log them in after registration
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, newLogin.LoginID),
                    new Claim(ClaimTypes.Role, "User"),
                    new Claim("CitizenID", newLogin.LoginID)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        // GET: Logout
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Login");
        }
    }
}
