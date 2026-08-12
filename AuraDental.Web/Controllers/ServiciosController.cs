using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using AuraDental.Aplicacion;
using AuraDental.Dominio.Entidades;
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
        public async Task<IActionResult> Crear(Servicio servicio, IFormFile? imagen)
        {
            if (_servicioService.ExisteNombre(servicio.Nombre))
            {
                ViewBag.Error = "Ya existe un servicio con ese nombre.";
                return View(servicio);
            }

            var (exito, mensaje) = _servicioService.Crear(servicio);

            if (!exito)
            {
                ViewBag.Error = mensaje;
                return View(servicio);
            }

            if (imagen != null && imagen.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await imagen.CopyToAsync(memoryStream);
                var extension = Path.GetExtension(imagen.FileName);

                var (exitoImagen, mensajeImagen) = _servicioService.SubirImagen(servicio.ServicioId, memoryStream.ToArray(), extension);

                if (!exitoImagen)
                {
                    TempData["Mensaje"] = $"Servicio creado, pero la imagen no se pudo guardar: {mensajeImagen}";
                }
            }

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
        public async Task<IActionResult> Editar(Servicio servicio, IFormFile? imagen)
        {
            if (_servicioService.ExisteNombre(servicio.Nombre, servicio.ServicioId))
            {
                ViewBag.Error = "Ya existe otro servicio con ese nombre.";
                return View(servicio);
            }

            _servicioService.Actualizar(servicio);

            if (imagen != null && imagen.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await imagen.CopyToAsync(memoryStream);
                var extension = Path.GetExtension(imagen.FileName);

                var (exitoImagen, mensajeImagen) = _servicioService.SubirImagen(servicio.ServicioId, memoryStream.ToArray(), extension);

                if (!exitoImagen)
                {
                    TempData["Mensaje"] = $"Servicio actualizado, pero la imagen no se pudo guardar: {mensajeImagen}";
                }
            }

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