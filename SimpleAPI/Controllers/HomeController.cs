using Microsoft.AspNetCore.Mvc;
namespace SimpleAPI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return Content("hi, i'm API!");
        }
    }
}
