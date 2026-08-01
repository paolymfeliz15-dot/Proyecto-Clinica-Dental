using Microsoft.AspNetCore.Mvc;
using AuraDental.Aplicacion;
using AuraDental.Web.Filters;

namespace AuraDental.Web.Controllers
{
    [SessionAuthorize(RolRequerido = "Paciente")]
    public class HistorialClinicoController : Controller
    {
        private readonly IExpedienteService _expedienteService;

        public HistorialClinicoController(IExpedienteService expedienteService)
        {
            _expedienteService = expedienteService;
        }

        public IActionResult Index()
        {
            var pacienteId = HttpContext.Session.GetInt32("UsuarioId")!.Value;
            var expedientes = _expedienteService.ObtenerPorPaciente(pacienteId);
            return View(expedientes);
        }
    }
}
