using Microsoft.AspNetCore.Mvc;

namespace TireSearchMVC.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}