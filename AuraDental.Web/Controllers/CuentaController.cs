using AuraDental.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using AuraDental.Aplicacion;
using AuraDental.Infraestructura;
using AuraDental.Dominio.Entidades;
using Microsoft.AspNetCore.Http;
using System.IO;

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
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registro(Usuario datosUsuario, string password)
        {
            var (exito, mensaje) = _authService.RegistrarPaciente(datosUsuario, password);

            if (!exito)
            {
                ViewBag.Error = mensaje;
                ViewBag.Paises = await _paisService.ObtenerPaisesAsync();
                return View(datosUsuario);
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
        public IActionResult CambiarNombreUsuario()
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null) return RedirectToAction("Login");

            ViewBag.NombreActual = HttpContext.Session.GetString("NombreCompleto");
            return View();
        }

        [HttpPost]
        public IActionResult CambiarNombreUsuario(string nuevoNombre)
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null) return RedirectToAction("Login");

            var (exito, mensaje) = _authService.CambiarNombreUsuario(usuarioId.Value, nuevoNombre);

            if (!exito)
            {
                ViewBag.Error = mensaje;
                ViewBag.NombreActual = HttpContext.Session.GetString("NombreCompleto");
                return View();
            }

            // Actualizamos la sesión para que el cambio se vea reflejado de inmediato en el navbar
            HttpContext.Session.SetString("NombreCompleto", nuevoNombre.Trim());

            ViewBag.Exito = mensaje;
            ViewBag.NombreActual = nuevoNombre.Trim();
            return View();
        }

        [HttpGet]
        public IActionResult EditarPerfil()
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
                return RedirectToAction("Login");

            var usuario = _context.Usuarios.Find(usuarioId.Value);
            if (usuario == null)
                return RedirectToAction("Login");

            return View(usuario);
        }

        [HttpPost]
        public IActionResult EditarPerfil(string nombreCompleto, string email)
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
                return RedirectToAction("Login");

            var (exito, mensaje) = _authService.ActualizarPerfil(usuarioId.Value, nombreCompleto, email);

            if (!exito)
            {
                ViewBag.Error = mensaje;
                var usuario = _context.Usuarios.Find(usuarioId.Value);
                return View(usuario);
            }

            // Actualizamos también el nombre guardado en la sesión,
            // para que se refleje de inmediato en el panel sin tener que reloguear
            HttpContext.Session.SetString("NombreCompleto", nombreCompleto);

            ViewBag.Exito = mensaje;
            var usuarioActualizado = _context.Usuarios.Find(usuarioId.Value);
            return View(usuarioActualizado);
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