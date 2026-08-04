using Microsoft.AspNetCore.Mvc;
using AuraDental.Aplicacion;
using AuraDental.Web.Filters;

namespace AuraDental.Web.Controllers
{
    [SessionAuthorize(RolRequerido = "Paciente")]
    public class ResenasController : Controller
    {
        private readonly IResenaService _resenaService;

        public ResenasController(IResenaService resenaService)
        {
            _resenaService = resenaService;
        }

        public IActionResult Crear() => View();

        [HttpPost]
        public IActionResult Crear(int calificacion, string comentario)
        {
            var pacienteId = HttpContext.Session.GetInt32("UsuarioId")!.Value;
            var (exito, mensaje) = _resenaService.Crear(pacienteId, calificacion, comentario);

            ViewBag.Exito = exito;
            ViewBag.Mensaje = mensaje;

            if (exito) return View();

            ViewBag.Error = mensaje;
            return View();
        }
    }
}