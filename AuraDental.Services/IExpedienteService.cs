using AuraDental.Dominio.Entidades;

namespace AuraDental.Aplicacion
{
    public interface IExpedienteService
    {
        List<Expediente> ObtenerPorPaciente(int pacienteId);
    }
}