using Microsoft.AspNetCore.Mvc;
using AuraDental.Services;
using AuraDental.Data.Entities;
using AuraDental.Web.Filters;

namespace AuraDental.Web.Controllers
{
    [SessionAuthorize(RolRequerido = "Asistente")]
    public class AgendaController : Controller
    {
        private readonly IAgendaService _agendaService;

        public AgendaController(IAgendaService agendaService)
        {
            _agendaService = agendaService;
        }

        // GET: /Agenda
        public IActionResult Index()
        {
            var bloques = _agendaService.ObtenerTodos();
            return View(bloques);
        }

        // GET: /Agenda/Detalles/5
        public IActionResult Detalles(int id)
        {
            var bloque = _agendaService.ObtenerPorId(id);
            if (bloque == null) return NotFound();

            return View(bloque);
        }

        // GET: /Agenda/Crear
        public IActionResult Crear()
        {
            return View();
        }

        // POST: /Agenda/Crear
        [HttpPost]
        public IActionResult Crear(BloqueAgenda bloque)
        {
            // El bloque queda asociado al asistente que inició sesión
            bloque.UsuarioId = HttpContext.Session.GetInt32("UsuarioId")!.Value;

            var (exito, mensaje) = _agendaService.Crear(bloque);

            if (!exito)
            {
                ViewBag.Error = mensaje;
                return View(bloque);
            }

            return RedirectToAction("Index");
        }

        // GET: /Agenda/Editar/5
        public IActionResult Editar(int id)
        {
            var bloque = _agendaService.ObtenerPorId(id);
            if (bloque == null) return NotFound();

            return View(bloque);
        }

        // POST: /Agenda/Editar/5
        [HttpPost]
        public IActionResult Editar(BloqueAgenda bloque)
        {
            var (exito, mensaje) = _agendaService.Actualizar(bloque);

            if (!exito)
            {
                ViewBag.Error = mensaje;
                return View(bloque);
            }

            return RedirectToAction("Index");
        }

        // POST: /Agenda/Eliminar/5
        [HttpPost]
        public IActionResult Eliminar(int id)
        {
            _agendaService.Eliminar(id);
            return RedirectToAction("Index");
        }
    }
}