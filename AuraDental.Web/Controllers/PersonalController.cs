using Microsoft.AspNetCore.Mvc;
using AuraDental.Services;
using AuraDental.Data;
using AuraDental.Data.Entities;
using AuraDental.Web.Filters;
using Microsoft.EntityFrameworkCore;

namespace AuraDental.Web.Controllers
{
    [SessionAuthorize(RolRequerido = "Administrador")]
    public class PersonalController : Controller
    {
        private readonly IPersonalService _personalService;
        private readonly AuraDentalDbContext _context;

        public PersonalController(IPersonalService personalService, AuraDentalDbContext context)
        {
            _personalService = personalService;
            _context = context;
        }

        // GET: /Personal
        public IActionResult Index()
        {
            var personal = _personalService.ObtenerTodos();
            return View(personal);
        }

        // GET: /Personal/Detalles/5
        public IActionResult Detalles(int id)
        {
            var usuario = _personalService.ObtenerPorId(id);
            if (usuario == null) return NotFound();

            return View(usuario);
        }

        // GET: /Personal/Crear
        public IActionResult Crear()
        {
            CargarRolesPersonal();
            return View();
        }

        // POST: /Personal/Crear
        [HttpPost]
        public IActionResult Crear(Usuario usuario, string password)
        {
            if (_personalService.ExisteEmail(usuario.Email))
            {
                ViewBag.Error = "Ese correo ya está registrado.";
                CargarRolesPersonal();
                return View(usuario);
            }

            _personalService.Crear(usuario, password);
            return RedirectToAction("Index");
        }

        // GET: /Personal/Editar/5
        public IActionResult Editar(int id)
        {
            var usuario = _personalService.ObtenerPorId(id);
            if (usuario == null) return NotFound();

            CargarRolesPersonal();
            return View(usuario);
        }

        // POST: /Personal/Editar/5
        [HttpPost]
        public IActionResult Editar(Usuario usuario)
        {
            if (_personalService.ExisteEmail(usuario.Email, usuario.UsuarioId))
            {
                ViewBag.Error = "Ese correo ya lo usa otro usuario.";
                CargarRolesPersonal();
                return View(usuario);
            }

            _personalService.Actualizar(usuario);
            return RedirectToAction("Index");
        }

        // POST: /Personal/CambiarEstado/5
        [HttpPost]
        public IActionResult CambiarEstado(int id, bool activo)
        {
            _personalService.CambiarEstado(id, activo);
            return RedirectToAction("Index");
        }

        private void CargarRolesPersonal()
        {
            // Solo se pueden crear Administradores o Asistentes desde aquí
            // (los Pacientes se registran desde /Cuenta/Registro)
            ViewBag.Roles = _context.Roles
                .Where(r => r.Nombre == "Administrador" || r.Nombre == "Asistente")
                .ToList();
        }
    }
}
