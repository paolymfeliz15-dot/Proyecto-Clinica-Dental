using Microsoft.AspNetCore.Mvc;
using AuraDental.Aplicacion;
using AuraDental.Web.Filters;

namespace AuraDental.Web.Controllers
{
    [SessionAuthorize(RolRequerido = "Paciente")]
    public class PacienteDashboardController : Controller
    {
        private readonly ICitaService _citaService;

        public PacienteDashboardController(ICitaService citaService)
        {
            _citaService = citaService;
        }

        public IActionResult Index()
        {
            ViewBag.NombreCompleto = HttpContext.Session.GetString("NombreCompleto");

            var pacienteId = HttpContext.Session.GetInt32("UsuarioId")!.Value;
            ViewBag.ProximaCita = _citaService.ObtenerProximaCitaParaRecordatorio(pacienteId);

            return View();
        }
    }
}