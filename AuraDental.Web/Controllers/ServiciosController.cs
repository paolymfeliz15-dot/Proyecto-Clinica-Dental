using Microsoft.AspNetCore.Mvc;
using AuraDental.Services;
using AuraDental.Data.Entities;
using AuraDental.Web.Filters;

namespace AuraDental.Web.Controllers
{
    [SessionAuthorize(RolRequerido = "Administrador")]
    public class ServiciosController : Controller
    {
        private readonly IServicioService _servicioService;

        public ServiciosController(IServicioService servicioService)
        {
            _servicioService = servicioService;
        }

        // GET: /Servicios
        public IActionResult Index()
        {
            var servicios = _servicioService.ObtenerTodos();
            return View(servicios);
        }

        // GET: /Servicios/Detalles/5
        public IActionResult Detalles(int id)
        {
            var servicio = _servicioService.ObtenerPorId(id);
            if (servicio == null) return NotFound();

            return View(servicio);
        }

        // GET: /Servicios/Crear
        public IActionResult Crear()
        {
            return View();
        }

        // POST: /Servicios/Crear
        [HttpPost]
        public IActionResult Crear(Servicio servicio)
        {
            if (_servicioService.ExisteNombre(servicio.Nombre))
            {
                ViewBag.Error = "Ya existe un servicio con ese nombre.";
                return View(servicio);
            }

            _servicioService.Crear(servicio);
            return RedirectToAction("Index");
        }

        // GET: /Servicios/Editar/5
        public IActionResult Editar(int id)
        {
            var servicio = _servicioService.ObtenerPorId(id);
            if (servicio == null) return NotFound();

            return View(servicio);
        }

        // POST: /Servicios/Editar/5
        [HttpPost]
        public IActionResult Editar(Servicio servicio)
        {
            if (_servicioService.ExisteNombre(servicio.Nombre, servicio.ServicioId))
            {
                ViewBag.Error = "Ya existe otro servicio con ese nombre.";
                return View(servicio);
            }

            _servicioService.Actualizar(servicio);
            return RedirectToAction("Index");
        }

        // POST: /Servicios/CambiarEstado/5
        [HttpPost]
        public IActionResult CambiarEstado(int id, bool activo)
        {
            _servicioService.CambiarEstado(id, activo);
            return RedirectToAction("Index");
        }
    }
}
