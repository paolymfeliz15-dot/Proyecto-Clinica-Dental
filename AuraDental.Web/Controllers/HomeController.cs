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
            // Mostramos hasta 3 servicios destacados en la vista previa del inicio;
            // el catálogo completo con imágenes llega en HU-20 (Sprint 5)
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