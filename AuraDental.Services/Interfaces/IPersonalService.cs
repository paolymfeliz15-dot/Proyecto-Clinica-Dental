using AuraDental.Aplicacion.Dtos;

namespace AuraDental.Aplicacion
{
    public interface IPersonalService
    {
        List<UsuarioResumenDto> ObtenerTodos();
        UsuarioResumenDto? ObtenerPorId(int id);
        bool ExisteEmail(string email, int? idExcluir = null);
        void Crear(PersonalDto datos);
        void Actualizar(PersonalDto datos);
        void CambiarEstado(int id, bool activo);
        bool ExisteCedula(string cedula, int? idExcluir = null);
        bool ExisteTelefono(string telefono, int? idExcluir = null);
    }
}