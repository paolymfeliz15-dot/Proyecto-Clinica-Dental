using AuraDental.Dominio.Entidades;

namespace AuraDental.Aplicacion
{
    public interface IAgendaService
    {
        List<BloqueAgenda> ObtenerTodos();
        List<BloqueAgenda> ObtenerPorFecha(DateTime fecha);
        List<BloqueAgenda> ObtenerDisponiblesPorServicio(int servicioId);
        BloqueAgenda? ObtenerPorId(int id);
        (bool exito, string mensaje) Crear(BloqueAgenda bloque);
        (bool exito, string mensaje) Actualizar(BloqueAgenda bloque);
        void Eliminar(int id);
        bool ExisteSolapamiento(DateTime fecha, TimeSpan horaInicio, TimeSpan horaFin, int? idExcluir = null);
    }
}
