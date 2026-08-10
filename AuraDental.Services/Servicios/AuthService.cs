using AuraDental.Aplicacion.Dtos;
using AuraDental.Aplicacion.Mappers;
using AuraDental.Dominio.Entidades;
using AuraDental.Dominio.Interfaces;
using AuraDental.Dominio.ObjetosValor;
using Microsoft.EntityFrameworkCore;

namespace AuraDental.Aplicacion
{
    public class AuthService : IAuthService
    {
        private readonly IRepository<Usuario> _usuarioRepository;
        private readonly IEmailService _emailService;

        public AuthService(IRepository<Usuario> usuarioRepository, IEmailService emailService)
        {
            _usuarioRepository = usuarioRepository;
            _emailService = emailService;
        }

        public bool ExisteEmail(string email)
        {
            return _usuarioRepository.Consultar().Any(u => u.Email == email);
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

            _usuarioRepository.Agregar(usuario);
            _usuarioRepository.GuardarCambios();

            return usuario;
        }

        public Usuario? ValidarCredenciales(string email, string password)
        {
            var usuario = _usuarioRepository.Consultar()
                .Include(u => u.Rol)
                .FirstOrDefault(u => u.Email == email && u.Activo);

            if (usuario == null)
                return null;

            bool passwordValida = BCrypt.Net.BCrypt.Verify(password, usuario.PasswordHash);
            return passwordValida ? usuario : null;
        }

        public (bool exito, string mensaje) CambiarPassword(int usuarioId, string passwordActual, string passwordNueva)
        {
            var usuario = _usuarioRepository.ObtenerPorId(usuarioId);
            if (usuario == null)
                return (false, "Usuario no encontrado.");

            if (!BCrypt.Net.BCrypt.Verify(passwordActual, usuario.PasswordHash))
                return (false, "La contraseña actual no es correcta.");

            if (passwordNueva.Length < 6)
                return (false, "La nueva contraseña debe tener al menos 6 caracteres.");

            usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(passwordNueva);
            _usuarioRepository.GuardarCambios();

            return (true, "Contraseña actualizada correctamente.");
        }

        public (bool exito, string mensaje) ActualizarPerfil(int usuarioId, EditarPerfilDto datos)
        {
            var usuario = _usuarioRepository.ObtenerPorId(usuarioId);
            if (usuario == null)
                return (false, "Usuario no encontrado.");

            var (emailValido, mensajeEmail, email) = Email.Crear(datos.Email);
            if (!emailValido)
                return (false, mensajeEmail);

            bool correoEnUso = _usuarioRepository.Consultar()
                .Any(u => u.Email == email!.Valor && u.UsuarioId != usuarioId);
            if (correoEnUso)
                return (false, "Ese correo ya lo está usando otra cuenta.");

            // La cédula es opcional para Administrador/Asistente creados antes de HU-21,
            // así que solo la validamos si viene con contenido
            if (!string.IsNullOrWhiteSpace(datos.Cedula))
            {
                var (cedulaValida, mensajeCedula, cedula) = Cedula.Crear(datos.Cedula);
                if (!cedulaValida)
                    return (false, mensajeCedula);

                bool cedulaEnUso = _usuarioRepository.Consultar()
                    .Any(u => u.Cedula == cedula!.Valor && u.UsuarioId != usuarioId);
                if (cedulaEnUso)
                    return (false, "Esa cédula ya está registrada en otra cuenta.");

                usuario.Cedula = cedula!.Valor;
            }

            usuario.NombreCompleto = datos.NombreCompleto;
            usuario.Apellidos = datos.Apellidos;
            usuario.Telefono = datos.Telefono;
            usuario.Email = email!.Valor;
            usuario.Direccion = datos.Direccion;
            usuario.Pais = datos.Pais;
            usuario.EstadoProvincia = datos.EstadoProvincia;
            usuario.Ciudad = datos.Ciudad;
            usuario.Sector = datos.Sector;

            _usuarioRepository.GuardarCambios();

            return (true, "Perfil actualizado correctamente.");
        }

        public (bool exito, string mensaje) CambiarNombreUsuario(int usuarioId, string nuevoNombre)
        {
            if (string.IsNullOrWhiteSpace(nuevoNombre) || nuevoNombre.Trim().Length < 2)
                return (false, "El nombre debe tener al menos 2 caracteres.");

            var usuario = _usuarioRepository.ObtenerPorId(usuarioId);
            if (usuario == null)
                return (false, "Usuario no encontrado.");

            usuario.NombreCompleto = nuevoNombre.Trim();
            _usuarioRepository.GuardarCambios();

            return (true, "Nombre de usuario actualizado correctamente.");
        }

        public (bool exito, string mensaje) RegistrarPaciente(RegistroPacienteDto datos)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(datos.NombreCompleto ?? "", @"^[A-Za-zÁÉÍÓÚáéíóúÑñÜü\s]+$"))
                return (false, "El nombre solo puede contener letras.");

            if (!System.Text.RegularExpressions.Regex.IsMatch(datos.Apellidos ?? "", @"^[A-Za-zÁÉÍÓÚáéíóúÑñÜü\s]+$"))
                return (false, "Los apellidos solo pueden contener letras.");

            var (emailValido, mensajeEmail, email) = Email.Crear(datos.Email);
            if (!emailValido)
                return (false, mensajeEmail);

            var (cedulaValida, mensajeCedula, cedula) = Cedula.Crear(datos.Cedula);
            if (!cedulaValida)
                return (false, mensajeCedula);

            if (!System.Text.RegularExpressions.Regex.IsMatch(datos.Telefono ?? "", @"^\d{3}-\d{3}-\d{4}$"))
                return (false, "El teléfono debe tener el formato 000-000-0000.");

            if (ExisteEmail(email!.Valor))
                return (false, "Ese correo ya está registrado.");

            if (_usuarioRepository.Consultar().Any(u => u.Cedula == cedula!.Valor))
                return (false, "Ya existe una cuenta registrada con esa cédula.");

            var usuario = UsuarioMapper.ARegistroPaciente(datos);
            usuario.Email = email.Valor;       // usamos el valor ya validado y normalizado
            usuario.Cedula = cedula!.Valor;    // en vez del texto crudo del formulario
            usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(datos.Password);
            usuario.RolId = 2;
            usuario.Activo = true;
            usuario.FechaCreacion = DateTime.Now;
            usuario.EmailVerificado = false;
            usuario.TokenVerificacion = Guid.NewGuid().ToString("N");
            usuario.TokenExpiracion = DateTime.Now.AddHours(24);

            _usuarioRepository.Agregar(usuario);
            _usuarioRepository.GuardarCambios();

            _ = _emailService.EnviarCorreoVerificacionAsync(usuario.Email, usuario.NombreCompleto, usuario.TokenVerificacion);

            return (true, "Registro exitoso. Revisa tu correo para verificar tu cuenta antes de iniciar sesión.");
        }

        public (bool exito, string mensaje) VerificarCorreo(string token)
        {
            var usuario = _usuarioRepository.Consultar().FirstOrDefault(u => u.TokenVerificacion == token);

            if (usuario == null)
                return (false, "Enlace de verificación inválido.");

            if (usuario.TokenExpiracion < DateTime.Now)
                return (false, "El enlace de verificación expiró. Solicita uno nuevo.");

            usuario.EmailVerificado = true;
            usuario.TokenVerificacion = null;
            usuario.TokenExpiracion = null;
            _usuarioRepository.GuardarCambios();

            return (true, "¡Correo verificado correctamente! Ya puedes iniciar sesión.");
        }

        public async Task<(bool exito, string mensaje)> ReenviarVerificacionAsync(string email)
        {
            var usuario = _usuarioRepository.Consultar().FirstOrDefault(u => u.Email == email);

            if (usuario == null)
                return (false, "No existe una cuenta con ese correo.");

            if (usuario.EmailVerificado)
                return (false, "Este correo ya está verificado.");

            usuario.TokenVerificacion = Guid.NewGuid().ToString("N");
            usuario.TokenExpiracion = DateTime.Now.AddHours(24);
            _usuarioRepository.GuardarCambios();

            var enviado = await _emailService.EnviarCorreoVerificacionAsync(usuario.Email, usuario.NombreCompleto, usuario.TokenVerificacion);

            return enviado
                ? (true, "Correo de verificación reenviado. Revisa tu bandeja.")
                : (false, "No se pudo enviar el correo. Intenta de nuevo más tarde.");
        }

        public (bool exito, string mensaje, string? rutaFoto) ActualizarFotoPerfil(int usuarioId, byte[] contenidoArchivo, string extension)
        {
            var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            const long tamanoMaximoBytes = 3 * 1024 * 1024;

            var usuario = _usuarioRepository.ObtenerPorId(usuarioId);
            if (usuario == null)
                return (false, "Usuario no encontrado.", null);

            extension = extension.ToLowerInvariant();
            if (!extensionesPermitidas.Contains(extension))
                return (false, "Formato de imagen no permitido. Usa JPG, PNG o WEBP.", null);

            if (contenidoArchivo.Length > tamanoMaximoBytes)
                return (false, "La imagen no puede superar los 3 MB.", null);

            var nombreArchivo = $"{usuarioId}_{Guid.NewGuid()}{extension}";
            var carpetaDestino = Path.Combine("wwwroot", "uploads", "perfiles");
            Directory.CreateDirectory(carpetaDestino);

            var rutaFisica = Path.Combine(carpetaDestino, nombreArchivo);
            File.WriteAllBytes(rutaFisica, contenidoArchivo);

            if (!string.IsNullOrWhiteSpace(usuario.FotoPerfilUrl))
            {
                var rutaAnterior = Path.Combine("wwwroot", usuario.FotoPerfilUrl.TrimStart('/'));
                if (File.Exists(rutaAnterior))
                    File.Delete(rutaAnterior);
            }

            var rutaWeb = $"/uploads/perfiles/{nombreArchivo}";
            usuario.FotoPerfilUrl = rutaWeb;
            _usuarioRepository.GuardarCambios();

            return (true, "Foto de perfil actualizada.", rutaWeb);
        }
    }
}