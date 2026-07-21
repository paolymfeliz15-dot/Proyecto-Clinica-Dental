using Microsoft.AspNetCore.Mvc;
using AuraDental.Services;
using AuraDental.Web.Filters;

namespace AuraDental.Web.Controllers
{
    [SessionAuthorize(RolRequerido = "Paciente")]
    public class CitasController : Controller
    {
        private readonly ICitaService _citaService;

        public CitasController(ICitaService citaService)
        {
            _citaService = citaService;
        }

        // GET: /Citas
        public IActionResult Index()
        {
            var pacienteId = HttpContext.Session.GetInt32("UsuarioId")!.Value;
            var citas = _citaService.ObtenerPorPaciente(pacienteId);
            return View(citas);
        }

        // POST: /Citas/Cancelar/5
        [HttpPost]
        public IActionResult Cancelar(int id)
        {
            var pacienteId = HttpContext.Session.GetInt32("UsuarioId")!.Value;
            var (exito, mensaje) = _citaService.Cancelar(id, pacienteId);

            TempData["Mensaje"] = mensaje;
            TempData["Exito"] = exito;

            return RedirectToAction("Index");
        }
    }
}