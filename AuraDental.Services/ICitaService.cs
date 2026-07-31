
using AuraDental.Dominio.Entidades;

namespace AuraDental.Aplicacion
{
    public interface ICitaService
    {
        List<Cita> ObtenerPorPaciente(int pacienteId);
        Cita? ObtenerPorId(int id);
        (bool exito, string mensaje) Cancelar(int citaId, int pacienteId);
        (bool exito, string mensaje) Agendar(int pacienteId, int servicioId, int bloqueAgendaId);
    }
}