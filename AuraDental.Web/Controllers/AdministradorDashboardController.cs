using AuraDental.Aplicacion;
using AuraDental.Web.Filters;
using Microsoft.AspNetCore.Mvc;

namespace AuraDental.Web.Controllers
{
    [SessionAuthorize(RolRequerido = "Administrador")]
    public class AdministradorDashboardController : Controller
    {
        [SessionAuthorize(RolRequerido = "Administrador")]
        public IActionResult Resenas([FromServices] IResenaService resenaService)
        {
            var resenas = resenaService.ObtenerTodas();
            return View(resenas);
        }
        public IActionResult Index()
        {
            ViewBag.NombreCompleto = HttpContext.Session.GetString("NombreCompleto");
            return View();
        }
    }
}