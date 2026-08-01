using System.Threading.Tasks;
using AuraDental.Dominio.Entidades;

namespace AuraDental.Aplicacion
{
    public interface IAuthService
    {
        bool ExisteEmail(string email);

        Usuario RegistrarUsuario(string nombreCompleto, string email, string password, int rolId);

        Usuario? ValidarCredenciales(string email, string password);

        (bool exito, string mensaje) CambiarPassword(int usuarioId, string passwordActual, string passwordNueva);

        (bool exito, string mensaje) ActualizarPerfil(int usuarioId, string nombreCompleto, string email);

        (bool exito, string mensaje) RegistrarPaciente(Usuario datosUsuario, string password);

        (bool exito, string mensaje) VerificarCorreo(string token);

        Task<(bool exito, string mensaje)> ReenviarVerificacionAsync(string email);

        (bool exito, string mensaje, string? rutaFoto) ActualizarFotoPerfil(int usuarioId, byte[] contenidoArchivo, string extension);

        // Si planeas usar 'CambiarNombreUsuario' desde el controller, agrégalo también:
        (bool exito, string mensaje) CambiarNombreUsuario(int usuarioId, string nuevoNombre);
    }
}