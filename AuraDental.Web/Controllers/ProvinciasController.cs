using Microsoft.AspNetCore.Mvc;
using AuraDental.Aplicacion;
using AuraDental.Dominio.Entidades;
using AuraDental.Web.Filters;

namespace AuraDental.Web.Controllers
{
    [SessionAuthorize(RolRequerido = "Administrador")]
    public class ProvinciasController : Controller
    {
        private readonly IProvinciaService _provinciaService;

        public ProvinciasController(IProvinciaService provinciaService)
        {
            _provinciaService = provinciaService;
        }

        public IActionResult Index()
        {
            var provincias = _provinciaService.ObtenerTodos();
            return View(provincias);
        }

        public IActionResult Detalles(int id)
        {
            var provincia = _provinciaService.ObtenerPorId(id);
            if (provincia == null) return NotFound();
            return View(provincia);
        }

        public IActionResult Crear() => View();

        [HttpPost]
        public IActionResult Crear(Provincia provincia)
        {
            if (_provinciaService.ExisteNombre(provincia.Nombre))
            {
                ViewBag.Error = "Ya existe una provincia con ese nombre.";
                return View(provincia);
            }

            _provinciaService.Crear(provincia);
            return RedirectToAction("Index");
        }

        public IActionResult Editar(int id)
        {
            var provincia = _provinciaService.ObtenerPorId(id);
            if (provincia == null) return NotFound();
            return View(provincia);
        }

        [HttpPost]
        public IActionResult Editar(Provincia provincia)
        {
            if (_provinciaService.ExisteNombre(provincia.Nombre, provincia.ProvinciaId))
            {
                ViewBag.Error = "Ya existe otra provincia con ese nombre.";
                return View(provincia);
            }

            _provinciaService.Actualizar(provincia);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult CambiarEstado(int id, bool activa)
        {
            _provinciaService.CambiarEstado(id, activa);
            return RedirectToAction("Index");
        }
    }
}