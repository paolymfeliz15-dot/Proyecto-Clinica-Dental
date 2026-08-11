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
        public async Task<IActionResult> Crear(PersonalDto datos)
        {
            var error = ValidarDuplicados(datos, idExcluir: null);

            if (error != null)
            {
                ViewBag.Error = error;
                CargarRolesPersonal();
                ViewBag.Paises = await _paisService.ObtenerPaisesAsync();
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

            ViewBag.EstadosActuales = !string.IsNullOrWhiteSpace(usuario.Pais)
                ? await _paisService.ObtenerEstadosAsync(usuario.Pais)
                : new List<string>();

            ViewBag.CiudadesActuales = !string.IsNullOrWhiteSpace(usuario.Pais) && !string.IsNullOrWhiteSpace(usuario.EstadoProvincia)
                ? await _paisService.ObtenerCiudadesAsync(usuario.Pais, usuario.EstadoProvincia)
                : new List<string>();

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
        public async Task<IActionResult> Editar(PersonalDto datos)
        {
            var error = ValidarDuplicados(datos, idExcluir: datos.UsuarioId);

            if (error != null)
            {
                ViewBag.Error = error;
                CargarRolesPersonal();
                ViewBag.Paises = await _paisService.ObtenerPaisesAsync();
                ViewBag.EstadosActuales = !string.IsNullOrWhiteSpace(datos.Pais) ? await _paisService.ObtenerEstadosAsync(datos.Pais) : new List<string>();
                ViewBag.CiudadesActuales = !string.IsNullOrWhiteSpace(datos.Pais) && !string.IsNullOrWhiteSpace(datos.EstadoProvincia) ? await _paisService.ObtenerCiudadesAsync(datos.Pais, datos.EstadoProvincia) : new List<string>();
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

        // Centraliza las 3 validaciones de duplicado, y devuelve el mensaje
        // de error correspondiente, o null si todo está en orden.
        private string? ValidarDuplicados(PersonalDto datos, int? idExcluir)
        {
            if (_personalService.ExisteEmail(datos.Email, idExcluir))
                return "Ese correo ya está registrado.";

            if (_personalService.ExisteCedula(datos.Cedula, idExcluir))
                return "Ya existe una cuenta registrada con esa cédula.";

            if (_personalService.ExisteTelefono(datos.Telefono, idExcluir))
                return "Ya existe una cuenta registrada con ese teléfono.";

            return null;
        }

        private void CargarRolesPersonal()
        {
            ViewBag.Roles = _context.Roles
                .Where(r => r.Nombre == "Administrador" || r.Nombre == "Asistente")
                .ToList();
        }
    }
}