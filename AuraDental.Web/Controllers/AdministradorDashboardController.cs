using Microsoft.AspNetCore.Mvc;
using AuraDental.Web.Filters;

namespace AuraDental.Web.Controllers
{
    [SessionAuthorize(RolRequerido = "Administrador")]
    public class AdministradorDashboardController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.NombreCompleto = HttpContext.Session.GetString("NombreCompleto");
            return View();
        }
    }
}