using AuraDental.Dominio.Entidades;
using AuraDental.Dominio.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AuraDental.Aplicacion
{
    public class ExpedienteService : IExpedienteService
    {
        private readonly IRepository<Expediente> _expedienteRepository;

        public ExpedienteService(IRepository<Expediente> expedienteRepository)
        {
            _expedienteRepository = expedienteRepository;
        }

        public List<Expediente> ObtenerPorPaciente(int pacienteId)
        {
            return _expedienteRepository.Consultar()
                .Include(e => e.RegistradoPor)
                .Include(e => e.Cita)
                    .ThenInclude(c => c!.Servicio)
                .Where(e => e.PacienteId == pacienteId)
                .OrderByDescending(e => e.FechaRegistro)
                .ToList();
        }
    }
}
