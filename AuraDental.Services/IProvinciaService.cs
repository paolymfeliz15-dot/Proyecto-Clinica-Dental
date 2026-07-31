using AuraDental.Dominio.Entidades;

namespace AuraDental.Aplicacion
{
    public interface IProvinciaService
    {
        List<Provincia> ObtenerTodos();
        Provincia? ObtenerPorId(int id);
        bool ExisteNombre(string nombre, int? idExcluir = null);
        void Crear(Provincia provincia);
        void Actualizar(Provincia provincia);
        void CambiarEstado(int id, bool activa);
    }
}