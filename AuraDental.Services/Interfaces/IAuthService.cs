using AuraDental.Aplicacion.Dtos;
using AuraDental.Dominio.Entidades;
using System.Threading.Tasks;

namespace AuraDental.Aplicacion
{
    public interface IAuthService
    {
        bool ExisteEmail(string email);

        Usuario RegistrarUsuario(string nombreCompleto, string email, string password, int rolId);

        Usuario? ValidarCredenciales(string email, string password);

        (bool exito, string mensaje) CambiarPassword(int usuarioId, string passwordActual, string passwordNueva);

        (bool exito, string mensaje) ActualizarPerfil(int usuarioId, EditarPerfilDto datos);

        (bool exito, string mensaje) RegistrarPaciente(RegistroPacienteDto datos);

        (bool exito, string mensaje) VerificarCorreo(string token);

        Task<(bool exito, string mensaje)> ReenviarVerificacionAsync(string email);

        (bool exito, string mensaje, string? rutaFoto) ActualizarFotoPerfil(int usuarioId, byte[] contenidoArchivo, string extension);
    }
}