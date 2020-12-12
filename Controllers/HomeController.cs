using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using LoginReg.Models;
using Microsoft.AspNetCore.Identity;
using LoginReg.Context;
using Microsoft.AspNetCore.Http;

namespace LoginReg.Controllers
{
    public class HomeController : Controller
    {
        private MyContext _context;

        public User LoggedInUser()
        {
            int? LoggedID = HttpContext.Session.GetInt32("LoggedInUser");
            User logged = _context.Users.FirstOrDefault(u => u.UserId == LoggedID);
            return logged;
        }

        public HomeController(MyContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("register")]
        public IActionResult Register(User reg)
        {
            //These two lines will hash our password for us.
            if (ModelState.IsValid)
            {
                if (_context.Users.Any(u => u.Email == reg.Email))
                {
                    ModelState.AddModelError("Email", "Email already in use!");
                    return View("Index");
                }
                else
                {
                    PasswordHasher<User> Hasher = new PasswordHasher<User>();
                    reg.Password = Hasher.HashPassword(reg, reg.Password);
                    _context.Users.Add(reg);
                    _context.SaveChanges();
                    var userInDb = _context.Users.FirstOrDefault(u => u.Email == reg.Email);
                    return View("Success", userInDb);
                }
            }
            else
            {
                return View("Index");
            }
        }

        [HttpPost("login")]
        public IActionResult Login(LoginUser log)
        {

            if (ModelState.IsValid)
            {
                // If inital ModelState is valid, query for a user with provided email
                var userInDb = _context.Users.FirstOrDefault(u => u.Email == log.LoginEmail);
                // If no user exists with provided email
                if (userInDb == null)
                {
                    // Add an error to ModelState and return to View!
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("Index");
                }

                // Initialize hasher object
                var hasher = new PasswordHasher<LoginUser>();

                // verify provided password against hash stored in db
                var result = hasher.VerifyHashedPassword(log, userInDb.Password, log.LoginPassword);

                // result can be compared to 0 for failure
                if (result == 0)
                {
                    ModelState.AddModelError("Email/Password", "Invalid Email/Password");
                    return View("Index");
                    // handle failure (this should be similar to how "existing email" is handled)
                }
                else
                {
                    HttpContext.Session.SetInt32("LoggedInUser", userInDb.UserId);
                    return RedirectToAction("Success");
                }
            }
            else
            {
                return View("Index");
            }
        }
        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
        [HttpGet("success")]
        public IActionResult Success()
        {
            return View("Success", LoggedInUser());
        }

















        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
