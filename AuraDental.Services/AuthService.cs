using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuraDental.Data;
using AuraDental.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuraDental.Services
{
    public class AuthService : IAuthService
    {
        private readonly AuraDentalDbContext _context;

        public AuthService(AuraDentalDbContext context)
        {
            _context = context;
        }

        public bool ExisteEmail(string email)
        {
            return _context.Usuarios.Any(u => u.Email == email);
        }

        public Usuario RegistrarUsuario(string nombreCompleto, string email, string password, int rolId)
        {
            var usuario = new Usuario
            {
                NombreCompleto = nombreCompleto,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                RolId = rolId,
                Activo = true
            };

            _context.Usuarios.Add(usuario);
            _context.SaveChanges();

            return usuario;
        }

        public Usuario? ValidarCredenciales(string email, string password)
        {
            var usuario = _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefault(u => u.Email == email && u.Activo);

            if (usuario == null)
                return null;

            bool passwordValida = BCrypt.Net.BCrypt.Verify(password, usuario.PasswordHash);

            return passwordValida ? usuario : null;
        }

        public (bool exito, string mensaje) CambiarPassword(int usuarioId, string passwordActual, string passwordNueva)
        {
            var usuario = _context.Usuarios.Find(usuarioId);
            if (usuario == null)
                return (false, "Usuario no encontrado.");

            if (!BCrypt.Net.BCrypt.Verify(passwordActual, usuario.PasswordHash))
                return (false, "La contraseña actual no es correcta.");

            if (passwordNueva.Length < 6)
                return (false, "La nueva contraseña debe tener al menos 6 caracteres.");

            usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(passwordNueva);
            _context.SaveChanges();

            return (true, "Contraseña actualizada correctamente.");
        }
    }
}