using AuraDental.Dominio.Entidades;

namespace AuraDental.Aplicacion
{
    public interface IExpedienteService
    {
        List<Expediente> ObtenerPorPaciente(int pacienteId);
        List<Cita> ObtenerCitasPendientesDeExpediente();
        (bool exito, string mensaje) Crear(int citaId, int registradoPorUsuarioId, string diagnostico, string tratamiento, string? observaciones);
    }
}