using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using hiking.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using hiking;
using Google.Maps;
using Google.Maps.Geocoding;


// Other using statements
namespace HomeController.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext;

        // here we can "inject" our context service into the constructor
        public HomeController(MyContext context)
        {
            dbContext = context;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            string UserInSession = HttpContext.Session.GetString("Email");
            if (UserInSession != null)
            {
                return RedirectToAction("Trails");
            }
            else
            {
                return View();
            }
        }

        [HttpPost("submit")]
        public IActionResult Submit(User newUser)
        {
            if (ModelState.IsValid)
            {
                if (dbContext.users.Any(u => u.Email == newUser.Email))
                {
                    // Manually add a ModelState error to the Email field, with provided
                    // error message
                    ModelState.AddModelError("Email", "Email already in use!");
                    // You may consider returning to the View at this point
                    return View("Index");
                }
                else
                {
                    PasswordHasher<User> Hasher = new PasswordHasher<User>();
                    newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
                    dbContext.Add(newUser);
                    dbContext.SaveChanges();
                    HttpContext.Session.SetString("Email", newUser.Email);
                    return RedirectToAction($"Trails");
                }
            }
            else
            {
                TempData["First Name"] = newUser.FirstName;
                TempData["Last Name"] = newUser.LastName;
                TempData["Email"] = newUser.Email;
                return View("Index");
            }
        }

        [HttpPost("submitlogin")]
        public IActionResult submitlogin(LoginUser retrievedUser)
        {
            if (ModelState.IsValid)
            {
                // If inital ModelState is valid, query for a user with provided email
                var userInDb = dbContext.users.FirstOrDefault(u => u.Email == retrievedUser.LoginEmail);
                // If no user exists with provided email
                if (userInDb == null)
                {
                    // Add an error to ModelState and return to View!
                    ModelState.AddModelError("LoginEmail", "You could not be logged in");
                    return View("Index");
                }

                // Initialize hasher object
                var hasher = new PasswordHasher<LoginUser>();

                // verify provided password against hash stored in dbcopy
                var result = hasher.VerifyHashedPassword(retrievedUser, userInDb.Password, retrievedUser.LoginPassword);

                // result can be compared to 0 for failure
                if (result == 0)
                {
                    ModelState.AddModelError("LoginEmail", "You could not be logged in");
                    return View("Index");
                }
                HttpContext.Session.SetString("Email", retrievedUser.LoginEmail);
                return RedirectToAction("Home");
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
            return View("Index");
        }
        [HttpGet("trails")]
        public IActionResult Trails()
        {
            string UserInSession = HttpContext.Session.GetString("Email");
            //always need to use YOUR_API_KEY for requests.  Do this in App_Start.
            if (UserInSession != null)
            {
                User retrievedUser = dbContext.users.FirstOrDefault(c => c.Email == UserInSession);
                ViewBag.User = retrievedUser;
                return View();
            }
            else
            {
                return RedirectToAction("Logout");
            }
        }
        [HttpPost("range")]
        public IActionResult Range()
        {
            return RedirectToAction("Trails");
        }

        [HttpPost("add")]
        public IActionResult Add(Trail likedTrail, int UserId, int id)
        {
            Favorite newFavorite = new Favorite();
            newFavorite.UserId = UserId;
            newFavorite.id = id;
            dbContext.favorites.Add(newFavorite);
            dbContext.SaveChanges();
            return Redirect ("Trails");
        }

        [HttpGet("favorites/{UserId}")]
        public IActionResult FavoriteHikes()
        {
            string UserInSession = HttpContext.Session.GetString("Email");
            if (UserInSession != null)
            {
                User UserFaves = dbContext.users
                .Include (mid => mid.faveHikes)
                .ThenInclude(t => t.myTrails)
                .FirstOrDefault(u => u.Email == UserInSession);
                return View(UserFaves);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
    }
}