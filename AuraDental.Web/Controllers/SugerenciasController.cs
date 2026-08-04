using Microsoft.AspNetCore.Mvc;
using AuraDental.Aplicacion;
using AuraDental.Web.Filters;

namespace AuraDental.Web.Controllers
{
    [SessionAuthorize(RolRequerido = "Paciente")]
    public class SugerenciasController : Controller
    {
        private readonly ISugerenciaService _sugerenciaService;

        public SugerenciasController(ISugerenciaService sugerenciaService)
        {
            _sugerenciaService = sugerenciaService;
        }

        public IActionResult Crear() => View();

        [HttpPost]
        public IActionResult Crear(string mensaje)
        {
            var pacienteId = HttpContext.Session.GetInt32("UsuarioId")!.Value;
            var (exito, resultado) = _sugerenciaService.Crear(pacienteId, mensaje);

            ViewBag.Exito = exito;
            ViewBag.Mensaje = resultado;

            if (!exito) ViewBag.Error = resultado;

            return View();
        }
    }
}
