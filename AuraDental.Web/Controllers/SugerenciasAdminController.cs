using Microsoft.AspNetCore.Mvc;
using AuraDental.Aplicacion;
using AuraDental.Web.Filters;

namespace AuraDental.Web.Controllers
{
    [SessionAuthorize(RolRequerido = "Administrador")]
    public class SugerenciasAdminController : Controller
    {
        private readonly ISugerenciaService _sugerenciaService;

        public SugerenciasAdminController(ISugerenciaService sugerenciaService)
        {
            _sugerenciaService = sugerenciaService;
        }

        public IActionResult Index()
        {
            var sugerencias = _sugerenciaService.ObtenerTodas();
            return View(sugerencias);
        }

        [HttpPost]
        public IActionResult MarcarLeida(int id)
        {
            _sugerenciaService.MarcarComoLeida(id);
            return RedirectToAction("Index");
        }
    }
}