using Microsoft.AspNetCore.Mvc;
using AuraDental.Aplicacion;

namespace AuraDental.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IServicioService _servicioService;

        public HomeController(IServicioService servicioService)
        {
            _servicioService = servicioService;
        }

        public IActionResult Index()
        {
            var rol = HttpContext.Session.GetString("Rol");

            if (rol == "Administrador") return RedirectToAction("Index", "AdministradorDashboard");
            if (rol == "Asistente") return RedirectToAction("Index", "AsistenteDashboard");
            if (rol == "Paciente") return RedirectToAction("Index", "PacienteDashboard");

            var servicios = _servicioService.ObtenerTodos()
                .Where(s => s.Activo)
                .Take(3)
                .ToList();

            return View(servicios);
        }

        public IActionResult PoliticaPrivacidad()
        {
            return View();
        }
    }
}