using Microsoft.AspNetCore.Mvc;

namespace AuraDental.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}