using AuraDental.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using AuraDental.Services;
using AuraDental.Data;

namespace AuraDental.Web.Controllers
{
    public class CuentaController : Controller
    {
        private readonly IAuthService _authService;
        private readonly AuraDentalDbContext _context;

        public CuentaController(IAuthService authService, AuraDentalDbContext context)
        {
            _authService = authService;
            _context = context;
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

            // Guardamos los datos clave del usuario en la sesión
            HttpContext.Session.SetInt32("UsuarioId", usuario.UsuarioId);
            HttpContext.Session.SetString("NombreCompleto", usuario.NombreCompleto);
            HttpContext.Session.SetString("Rol", usuario.Rol.Nombre);

            // Redirección según el rol
            return usuario.Rol.Nombre switch
            {
                "Administrador" => RedirectToAction("Index", "AdministradorDashboard"),
                "Asistente" => RedirectToAction("Index", "AsistenteDashboard"),
                "Paciente" => RedirectToAction("Index", "PacienteDashboard"),
                _ => RedirectToAction("Login")
            };
        }

        [HttpGet]
        public IActionResult Registro()
        {
            ViewBag.Roles = _context.Roles.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Registro(string nombreCompleto, string email, string password, int rolId)
        {
            if (_authService.ExisteEmail(email))
            {
                ViewBag.Error = "Ese correo ya está registrado.";
                ViewBag.Roles = _context.Roles.ToList();
                return View();
            }

            _authService.RegistrarUsuario(nombreCompleto, email, password, rolId);

            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
