using Microsoft.AspNetCore.Mvc;
using AuraDental.Web.Filters;

namespace AuraDental.Web.Controllers
{
    [SessionAuthorize(RolRequerido = "Paciente")]
    public class PacienteDashboardController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.NombreCompleto = HttpContext.Session.GetString("NombreCompleto");
            return View();
        }
    }
}