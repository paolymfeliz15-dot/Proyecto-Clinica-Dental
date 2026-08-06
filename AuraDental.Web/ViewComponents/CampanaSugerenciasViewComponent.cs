using Microsoft.AspNetCore.Mvc;
using AuraDental.Aplicacion;

namespace AuraDental.Web.ViewComponents
{
    public class CampanaSugerenciasViewComponent : ViewComponent
    {
        private readonly ISugerenciaService _sugerenciaService;

        public CampanaSugerenciasViewComponent(ISugerenciaService sugerenciaService)
        {
            _sugerenciaService = sugerenciaService;
        }

        public IViewComponentResult Invoke()
        {
            var noLeidas = _sugerenciaService.ContarNoLeidas();
            return View(model: noLeidas);
        }
    }
}