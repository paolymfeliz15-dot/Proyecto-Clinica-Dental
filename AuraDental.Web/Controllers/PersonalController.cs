using Microsoft.AspNetCore.Mvc;
using AuraDental.Aplicacion;
using AuraDental.Aplicacion.Dtos;
using AuraDental.Infraestructura;
using AuraDental.Web.Filters;

namespace AuraDental.Web.Controllers
{
    [SessionAuthorize(RolRequerido = "Administrador")]
    public class PersonalController : Controller
    {
        private readonly IPersonalService _personalService;
        private readonly AuraDentalDbContext _context;
        private readonly IPaisService _paisService;

        public PersonalController(IPersonalService personalService, AuraDentalDbContext context, IPaisService paisService)
        {
            _personalService = personalService;
            _context = context;
            _paisService = paisService;
        }

        public IActionResult Index()
        {
            var personal = _personalService.ObtenerTodos();
            return View(personal);
        }

        public IActionResult Detalles(int id)
        {
            var usuario = _personalService.ObtenerPorId(id);
            if (usuario == null) return NotFound();
            return View(usuario);
        }

        public async Task<IActionResult> Crear()
        {
            CargarRolesPersonal();
            ViewBag.Paises = await _paisService.ObtenerPaisesAsync();
            return View(new PersonalDto());
        }

        [HttpPost]
        public IActionResult Crear(PersonalDto datos)
        {
            if (_personalService.ExisteEmail(datos.Email))
            {
                ViewBag.Error = "Ese correo ya está registrado.";
                CargarRolesPersonal();
                return View(datos);
            }

            _personalService.Crear(datos);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Editar(int id)
        {
            var usuario = _personalService.ObtenerPorId(id);
            if (usuario == null) return NotFound();

            CargarRolesPersonal();
            ViewBag.Paises = await _paisService.ObtenerPaisesAsync();

            if (!string.IsNullOrWhiteSpace(usuario.Pais))
                ViewBag.EstadosActuales = await _paisService.ObtenerEstadosAsync(usuario.Pais);
            else
                ViewBag.EstadosActuales = new List<string>();

            if (!string.IsNullOrWhiteSpace(usuario.Pais) && !string.IsNullOrWhiteSpace(usuario.EstadoProvincia))
                ViewBag.CiudadesActuales = await _paisService.ObtenerCiudadesAsync(usuario.Pais, usuario.EstadoProvincia);
            else
                ViewBag.CiudadesActuales = new List<string>();

            // Convertimos el DTO de salida en el DTO de entrada que el formulario necesita
            var datos = new PersonalDto
            {
                UsuarioId = usuario.UsuarioId,
                NombreCompleto = usuario.NombreCompleto,
                Apellidos = usuario.Apellidos,
                Cedula = usuario.Cedula,
                Telefono = usuario.Telefono,
                Email = usuario.Email,
                Direccion = usuario.Direccion,
                Pais = usuario.Pais,
                EstadoProvincia = usuario.EstadoProvincia,
                Ciudad = usuario.Ciudad,
                Sector = usuario.Sector
            };

            return View(datos);
        }

        [HttpPost]
        public IActionResult Editar(PersonalDto datos)
        {
            if (_personalService.ExisteEmail(datos.Email, datos.UsuarioId))
            {
                ViewBag.Error = "Ese correo ya lo usa otro usuario.";
                CargarRolesPersonal();
                return View(datos);
            }

            _personalService.Actualizar(datos);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult CambiarEstado(int id, bool activo)
        {
            _personalService.CambiarEstado(id, activo);
            return RedirectToAction("Index");
        }

        private void CargarRolesPersonal()
        {
            ViewBag.Roles = _context.Roles
                .Where(r => r.Nombre == "Administrador" || r.Nombre == "Asistente")
                .ToList();
        }
    }
}