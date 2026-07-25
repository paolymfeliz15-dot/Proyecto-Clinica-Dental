using Microsoft.AspNetCore.Mvc;
using AuraDental.Services;
using AuraDental.Web.Filters;

namespace AuraDental.Web.Controllers
{
    [SessionAuthorize(RolRequerido = "Paciente")]
    public class DisponibilidadController : Controller
    {
        private readonly IAgendaService _agendaService;
        private readonly IServicioService _servicioService;

        public DisponibilidadController(IAgendaService agendaService, IServicioService servicioService)
        {
            _agendaService = agendaService;
            _servicioService = servicioService;
        }

        // GET: /Disponibilidad
        public IActionResult Index()
        {
            // Solo servicios activos pueden ser elegidos para consultar disponibilidad
            var servicios = _servicioService.ObtenerTodos()
                .Where(s => s.Activo)
                .ToList();

            return View(servicios);
        }

        // GET: /Disponibilidad/Consultar?servicioId=3
        public IActionResult Consultar(int servicioId)
        {
            var servicio = _servicioService.ObtenerPorId(servicioId);
            if (servicio == null) return NotFound();

            var bloques = _agendaService.ObtenerDisponiblesPorServicio(servicioId);

            ViewBag.Servicio = servicio;
            return View(bloques);
        }
    }
}