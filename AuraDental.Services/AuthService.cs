using System;
using System.Collections.Generic;
using System.IO;
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

        private readonly string[] _extensionesPermitidas = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long TamanoMaximoBytes = 3 * 1024 * 1024; // 3 MB

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

        public (bool exito, string mensaje) RegistrarPaciente(Usuario datosUsuario, string password)
        {
            if (ExisteEmail(datosUsuario.Email))
                return (false, "Ese correo ya está registrado.");

            if (!string.IsNullOrWhiteSpace(datosUsuario.Cedula) &&
                _context.Usuarios.Any(u => u.Cedula == datosUsuario.Cedula))
                return (false, "Ya existe una cuenta registrada con esa cédula.");

            datosUsuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            datosUsuario.RolId = 2; // Paciente, siempre fijo
            datosUsuario.Activo = true;
            datosUsuario.FechaCreacion = DateTime.Now;

            _context.Usuarios.Add(datosUsuario);
            _context.SaveChanges();

            return (true, "Registro exitoso.");
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

        public (bool exito, string mensaje) ActualizarPerfil(int usuarioId, string nombreCompleto, string email)
        {
            var usuario = _context.Usuarios.Find(usuarioId);
            if (usuario == null)
                return (false, "Usuario no encontrado.");

            bool correoEnUso = _context.Usuarios
                .Any(u => u.Email == email && u.UsuarioId != usuarioId);

            if (correoEnUso)
                return (false, "Ese correo ya lo está usando otra cuenta.");

            usuario.NombreCompleto = nombreCompleto;
            usuario.Email = email;
            _context.SaveChanges();

            return (true, "Perfil actualizado correctamente.");
        }

        public (bool exito, string mensaje, string? rutaFoto) ActualizarFotoPerfil(int usuarioId, byte[] contenidoArchivo, string extension)
        {
            var usuario = _context.Usuarios.Find(usuarioId);
            if (usuario == null)
                return (false, "Usuario no encontrado.", null);

            extension = extension.ToLowerInvariant();
            if (!_extensionesPermitidas.Contains(extension))
                return (false, "Formato de imagen no permitido. Usa JPG, PNG o WEBP.", null);

            if (contenidoArchivo.Length > TamanoMaximoBytes)
                return (false, "La imagen no puede superar los 3 MB.", null);

            var nombreArchivo = $"{usuarioId}_{Guid.NewGuid()}{extension}";
            var carpetaDestino = Path.Combine("wwwroot", "uploads", "perfiles");
            Directory.CreateDirectory(carpetaDestino);

            var rutaFisica = Path.Combine(carpetaDestino, nombreArchivo);
            File.WriteAllBytes(rutaFisica, contenidoArchivo);

            // Borramos la foto anterior si existía, para no acumular archivos huérfanos
            if (!string.IsNullOrWhiteSpace(usuario.FotoPerfilUrl))
            {
                var rutaAnterior = Path.Combine("wwwroot", usuario.FotoPerfilUrl.TrimStart('/'));
                if (File.Exists(rutaAnterior))
                    File.Delete(rutaAnterior);
            }

            var rutaWeb = $"/uploads/perfiles/{nombreArchivo}";
            usuario.FotoPerfilUrl = rutaWeb;
            _context.SaveChanges();

            return (true, "Foto de perfil actualizada.", rutaWeb);
        }
    }
}