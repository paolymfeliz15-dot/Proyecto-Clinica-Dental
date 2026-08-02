using Microsoft.AspNetCore.Mvc;
using AuraDental.Aplicacion;
using AuraDental.Web.Filters;

namespace AuraDental.Web.Controllers
{
    [SessionAuthorize(RolRequerido = "Administrador")]
    public class ReportesController : Controller
    {
        private readonly ICitaService _citaService;

        public ReportesController(ICitaService citaService)
        {
            _citaService = citaService;
        }

        // GET: /Reportes/Citas
        public IActionResult Citas(DateTime? desde, DateTime? hasta)
        {
            // Por defecto, mostramos los últimos 7 días si no se especifica un rango
            var fechaDesde = desde ?? DateTime.Today.AddDays(-7);
            var fechaHasta = hasta ?? DateTime.Today;

            var citas = _citaService.ObtenerPorRangoFechas(fechaDesde, fechaHasta);

            ViewBag.Desde = fechaDesde.ToString("yyyy-MM-dd");
            ViewBag.Hasta = fechaHasta.ToString("yyyy-MM-dd");
            ViewBag.Total = citas.Count;

            return View(citas);
        }
    }
}