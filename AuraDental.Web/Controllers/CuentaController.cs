using AuraDental.Aplicacion;
using AuraDental.Aplicacion.Dtos;
using AuraDental.Infraestructura;
using Microsoft.AspNetCore.Mvc;

namespace AuraDental.Web.Controllers
{
    public class CuentaController : Controller
    {
        private readonly IAuthService _authService;
        private readonly AuraDentalDbContext _context;
        private readonly IPaisService _paisService;

        public CuentaController(IAuthService authService, AuraDentalDbContext context, IPaisService paisService)
        {
            _authService = authService;
            _context = context;
            _paisService = paisService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult AccesoDenegado()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var usuario = _authService.ValidarCredenciales(email, password);

            if (usuario == null)
            {
                ViewBag.Error = "Correo o contraseña incorrectos.";
                return View();
            }

            if (!usuario.EmailVerificado)
            {
                ViewBag.Error = "Debes verificar tu correo antes de iniciar sesión.";
                ViewBag.MostrarReenviar = true;
                ViewBag.EmailPendiente = email;
                return View();
            }

            // Guardamos los datos clave del usuario en la sesión
            HttpContext.Session.SetInt32("UsuarioId", usuario.UsuarioId);
            HttpContext.Session.SetString("NombreCompleto", usuario.NombreCompleto);
            HttpContext.Session.SetString("Rol", usuario.Rol.Nombre);

            if (!string.IsNullOrWhiteSpace(usuario.FotoPerfilUrl))
                HttpContext.Session.SetString("FotoPerfilUrl", usuario.FotoPerfilUrl);

            // Redirección según el rol
            return usuario.Rol.Nombre switch
            {
                "Administrador" => RedirectToAction("Index", "AdministradorDashboard"),
                "Asistente" => RedirectToAction("Index", "AsistenteDashboard"),
                "Paciente" => RedirectToAction("Index", "PacienteDashboard"),
                _ => RedirectToAction("Login")
            };
        }

        public IActionResult VerificarCorreo(string token)
        {
            var (exito, mensaje) = _authService.VerificarCorreo(token);
            ViewBag.Exito =  exito;
            ViewBag.Mensaje = mensaje;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ReenviarVerificacion(string email)
        {
            var (exito, mensaje) = await _authService.ReenviarVerificacionAsync(email);
            TempData["MensajeReenvio"] = mensaje;
            return RedirectToAction("Login");
        }

        [HttpGet]
        public async Task<IActionResult> Registro()
        {
            ViewBag.Paises = await _paisService.ObtenerPaisesAsync();
            return View(new RegistroPacienteDto());
        }

        [HttpPost]
        public async Task<IActionResult> Registro(RegistroPacienteDto datos)
        {
            var (exito, mensaje) = _authService.RegistrarPaciente(datos);

            if (!exito)
            {
                ViewBag.Error = mensaje;
                ViewBag.Paises = await _paisService.ObtenerPaisesAsync();
                return View(datos);
            }

            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult CambiarPassword()
        {
            if (HttpContext.Session.GetInt32("UsuarioId") == null)
                return RedirectToAction("Login");

            return View();
        }

        [HttpPost]
        public IActionResult CambiarPassword(string passwordActual, string passwordNueva)
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
                return RedirectToAction("Login");

            var (exito, mensaje) = _authService.CambiarPassword(usuarioId.Value, passwordActual, passwordNueva);
            if (!exito)
            {
                ViewBag.Error = mensaje;
                return View();
            }

            ViewBag.Exito = mensaje;
            return View();
        }

        [HttpGet]
        public IActionResult Perfil()
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null) return RedirectToAction("Login");

            var usuario = _context.Usuarios.Find(usuarioId.Value);
            if (usuario == null) return RedirectToAction("Login");

            return View(usuario);
        }

        [HttpGet]
        public async Task<IActionResult> EditarPerfil()
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null) return RedirectToAction("Login");

            var usuario = _context.Usuarios.Find(usuarioId.Value);
            if (usuario == null) return RedirectToAction("Login");

            ViewBag.Paises = await _paisService.ObtenerPaisesAsync();

            ViewBag.EstadosActuales = !string.IsNullOrWhiteSpace(usuario.Pais)
                ? await _paisService.ObtenerEstadosAsync(usuario.Pais)
                : new List<string>();

            ViewBag.CiudadesActuales = !string.IsNullOrWhiteSpace(usuario.Pais) && !string.IsNullOrWhiteSpace(usuario.EstadoProvincia)
                ? await _paisService.ObtenerCiudadesAsync(usuario.Pais, usuario.EstadoProvincia)
                : new List<string>();

            var datos = new EditarPerfilDto
            {
                NombreCompleto = usuario.NombreCompleto,
                Apellidos = usuario.Apellidos ?? string.Empty,
                Cedula = usuario.Cedula ?? string.Empty,
                Telefono = usuario.Telefono ?? string.Empty,
                Email = usuario.Email,
                Direccion = usuario.Direccion ?? string.Empty,
                Pais = usuario.Pais ?? string.Empty,
                EstadoProvincia = usuario.EstadoProvincia ?? string.Empty,
                Ciudad = usuario.Ciudad ?? string.Empty,
                Sector = usuario.Sector ?? string.Empty
            };

            // El modelo de la vista también necesita la foto actual y el UsuarioId para el formulario de foto
            ViewBag.FotoPerfilUrl = usuario.FotoPerfilUrl;

            return View(datos);
        }

        [HttpPost]
        public async Task<IActionResult> EditarPerfil(EditarPerfilDto datos)
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null) return RedirectToAction("Login");

            var (exito, mensaje) = _authService.ActualizarPerfil(usuarioId.Value, datos);

            if (!exito)
            {
                ViewBag.Error = mensaje;
                ViewBag.Paises = await _paisService.ObtenerPaisesAsync();
                ViewBag.EstadosActuales = !string.IsNullOrWhiteSpace(datos.Pais) ? await _paisService.ObtenerEstadosAsync(datos.Pais) : new List<string>();
                ViewBag.CiudadesActuales = !string.IsNullOrWhiteSpace(datos.Pais) && !string.IsNullOrWhiteSpace(datos.EstadoProvincia) ? await _paisService.ObtenerCiudadesAsync(datos.Pais, datos.EstadoProvincia) : new List<string>();
                var usuarioActual = _context.Usuarios.Find(usuarioId.Value);
                ViewBag.FotoPerfilUrl = usuarioActual?.FotoPerfilUrl;
                return View(datos);
            }

            // Sincronizamos el nombre en sesión, para que se refleje de inmediato en el navbar
            HttpContext.Session.SetString("NombreCompleto", datos.NombreCompleto);

            ViewBag.Exito = mensaje;
            ViewBag.Paises = await _paisService.ObtenerPaisesAsync();
            ViewBag.EstadosActuales = await _paisService.ObtenerEstadosAsync(datos.Pais);
            ViewBag.CiudadesActuales = await _paisService.ObtenerCiudadesAsync(datos.Pais, datos.EstadoProvincia);
            var usuarioRecargado = _context.Usuarios.Find(usuarioId.Value);
            ViewBag.FotoPerfilUrl = usuarioRecargado?.FotoPerfilUrl;

            return View(datos);
        }

        [HttpPost]
        public async Task<IActionResult> ActualizarFotoPerfil(IFormFile foto)
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
                return RedirectToAction("Login");

            if (foto == null || foto.Length == 0)
            {
                TempData["ErrorFoto"] = "Debes seleccionar una imagen.";
                return RedirectToAction("EditarPerfil");
            }

            using var memoryStream = new MemoryStream();
            await foto.CopyToAsync(memoryStream);
            var extension = Path.GetExtension(foto.FileName);

            var (exito, mensaje, rutaFoto) = _authService.ActualizarFotoPerfil(usuarioId.Value, memoryStream.ToArray(), extension);

            if (exito && rutaFoto != null)
            {
                HttpContext.Session.SetString("FotoPerfilUrl", rutaFoto);
            }

            TempData["ErrorFoto"] = exito ? null : mensaje;
            return RedirectToAction("EditarPerfil");
        }
    }
}