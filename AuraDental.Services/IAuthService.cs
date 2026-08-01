using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuraDental.Dominio.Entidades;

namespace AuraDental.Aplicacion
{
    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        // (Nota: Asegúrate de mantener aquí las demás dependencias e inyecciones que ya tenga tu clase AuthService)

        public AuthService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public Usuario? ValidarCredenciales(string email, string password)
        {
            // Lógica existente...
            throw new NotImplementedException();
        }

        public Usuario RegistrarUsuario(string nombreCompleto, string email, string password, int rolId)
        {
            // Lógica existente...
            throw new NotImplementedException();
        }

        public bool ExisteEmail(string email)
        {
            // Lógica existente...
            throw new NotImplementedException();
        }

        public (bool exito, string mensaje) CambiarPassword(int usuarioId, string passwordActual, string passwordNueva)
        {
            // Lógica existente...
            throw new NotImplementedException();
        }

        public (bool exito, string mensaje) ActualizarPerfil(int usuarioId, string nombreCompleto, string email)
        {
            // Lógica existente...
            throw new NotImplementedException();
        }

        // MÉTODO AGREGADO SEGÚN EL MANDATO:
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

        public (bool exito, string mensaje) RegistrarPaciente(Usuario datosUsuario, string password)
        {
            // Lógica existente...
            throw new NotImplementedException();
        }

        public (bool exito, string mensaje, string? rutaFoto) ActualizarFotoPerfil(int usuarioId, byte[] contenidoArchivo, string extension)
        {
            // Lógica existente...
            throw new NotImplementedException();
        }

        public (bool exito, string mensaje) VerificarCorreo(string token)
        {
            // Lógica existente...
            throw new NotImplementedException();
        }

        public async Task<(bool exito, string mensaje)> ReenviarVerificacionAsync(string email)
        {
            // Lógica existente...
            throw new NotImplementedException();
        }
    }
}