using AuraDental.Data.Entities;

namespace AuraDental.Services
{
    public interface IPersonalService
    {
        List<Usuario> ObtenerTodos();
        Usuario? ObtenerPorId(int id);
        void Crear(Usuario usuario, string password);
        void Actualizar(Usuario usuario);
        void CambiarEstado(int id, bool activo);
        bool ExisteEmail(string email, int? idExcluir = null);
    }
}