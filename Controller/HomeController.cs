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
                return RedirectToAction("Home");
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
                    return RedirectToAction($"Home");
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
            GoogleSigned.AssignAllServices(new GoogleSigned ("AIzaSyDFbGNmXKLwZV6NqhluyzrGQT99zeUCg6w"));

            //always need to use YOUR_API_KEY for requests.  Do this in App_Start.
            var request = new GeocodingRequest();
            request.Address = "1600 Pennsylvania Ave NW, Washington, DC 20500, USA";
            var response = new GeocodingService().GetResponse(request);

            //The GeocodingService class submits the request to the API web service, and returns the
            //response strongly typed as a GeocodeResponse object which may contain zero, one or more results.

            //Assuming we received at least one result, let's get some of its properties:

            if (response.Status == ServiceResponseStatus.Ok && response.Results.Count() > 0)
            {

                var result = response.Results.First();

                Console.WriteLine("Full Address: " + result.FormattedAddress);
                // "1600 Pennsylvania Ave NW, Washington, DC 20500, USA"
                Console.WriteLine("Latitude: " + result.Geometry.Location.Latitude); // 38.8976633
                Console.WriteLine("Longitude: " + result.Geometry.Location.Longitude); // -77.0365739
                Console.WriteLine();
                ViewBag.Lati = result.Geometry.Location.Latitude;
                ViewBag.Long = result.Geometry.Location.Longitude;

            }
            else
            {
                Console.WriteLine("Unable to geocode.  Status={0} and ErrorMessage={1}", response.Status, response.ErrorMessage);
            }
            return View();
        }
    }
}