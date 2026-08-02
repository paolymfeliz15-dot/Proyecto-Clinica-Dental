
using AuraDental.Dominio.Entidades;

namespace AuraDental.Aplicacion
{
    public interface ICitaService
    {
        List<Cita> ObtenerPorPaciente(int pacienteId);
        List<Cita> ObtenerPorRangoFechas(DateTime desde, DateTime hasta);
        Cita? ObtenerPorId(int id);
        (bool exito, string mensaje) Cancelar(int citaId, int pacienteId);
        (bool exito, string mensaje) Agendar(int pacienteId, int servicioId, int bloqueAgendaId);
    }
}