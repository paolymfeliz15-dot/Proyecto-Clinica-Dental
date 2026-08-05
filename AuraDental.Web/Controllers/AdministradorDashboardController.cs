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

        [SessionAuthorize(RolRequerido = "Administrador")]
        public IActionResult Graficos([FromServices] IDashboardService dashboardService)
        {
            var estadisticas = dashboardService.ObtenerEstadisticas();
            return View(estadisticas);
        }

        public IActionResult Index()
        {
            ViewBag.NombreCompleto = HttpContext.Session.GetString("NombreCompleto");
            return View();
        }
    }
}