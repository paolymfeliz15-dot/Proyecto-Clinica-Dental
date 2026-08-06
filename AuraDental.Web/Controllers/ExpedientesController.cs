using Microsoft.AspNetCore.Mvc;
using AuraDental.Aplicacion;
using AuraDental.Web.Filters;

namespace AuraDental.Web.Controllers
{
    [SessionAuthorize(RolRequerido = "Asistente")]
    public class ExpedientesController : Controller
    {
        private readonly IExpedienteService _expedienteService;

        public ExpedientesController(IExpedienteService expedienteService)
        {
            _expedienteService = expedienteService;
        }

        // GET: /Expedientes -> lista de citas pendientes de registrar
        public IActionResult Index()
        {
            var citasPendientes = _expedienteService.ObtenerCitasPendientesDeExpediente();
            return View(citasPendientes);
        }

        // GET: /Expedientes/Crear/5
        public IActionResult Crear(int citaId)
        {
            ViewBag.CitaId = citaId;
            return View();
        }

        // POST: /Expedientes/Crear
        [HttpPost]
        public IActionResult Crear(int citaId, string diagnostico, string tratamiento, string? observaciones)
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId")!.Value;
            var (exito, mensaje) = _expedienteService.Crear(citaId, usuarioId, diagnostico, tratamiento, observaciones);

            if (!exito)
            {
                ViewBag.Error = mensaje;
                ViewBag.CitaId = citaId;
                return View();
            }

            TempData["Mensaje"] = mensaje;
            return RedirectToAction("Index");
        }
    }
}