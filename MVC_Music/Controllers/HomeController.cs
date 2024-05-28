using Microsoft.AspNetCore.Mvc;
using MVC_Music.Models;
using System.Diagnostics;

namespace MVC_Music.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger; // Injects the logger into the controller
        }

        // GET: /Home/Index
        public IActionResult Index()
        {
            return View(); // Returns the view for the home page
        }

        // GET: /Home/Privacy
        public IActionResult Privacy()
        {
            return View(); // Returns the view for the privacy page
        }

        // GET: /Home/Error
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // Returns the view for the error page with an error model containing the current request ID
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
