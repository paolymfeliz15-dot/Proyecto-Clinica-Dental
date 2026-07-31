using Microsoft.AspNetCore.Mvc;
using AuraDental.Aplicacion;

namespace AuraDental.Web.Controllers
{
    // Este controlador expone nuestra propia API interna, que a su vez llama a
    // CountriesNow desde el servidor. El JavaScript del navegador nunca llama
    // directamente a la API externa; siempre pasa por aquí.
    public class LocalizacionController : Controller
    {
        private readonly IPaisService _paisService;

        public LocalizacionController(IPaisService paisService)
        {
            _paisService = paisService;
        }

        [HttpGet]
        public async Task<JsonResult> Estados(string pais)
        {
            var estados = await _paisService.ObtenerEstadosAsync(pais);
            return Json(estados);
        }

        [HttpGet]
        public async Task<JsonResult> Ciudades(string pais, string estado)
        {
            var ciudades = await _paisService.ObtenerCiudadesAsync(pais, estado);
            return Json(ciudades);
        }
    }
}