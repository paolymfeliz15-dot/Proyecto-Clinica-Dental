using AuraDental.Dominio.Entidades;
using AuraDental.Dominio.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AuraDental.Aplicacion
{
    public class ExpedienteService : IExpedienteService
    {
        private readonly IRepository<Expediente> _expedienteRepository;
        private readonly IRepository<Cita> _citaRepository;

        public ExpedienteService(IRepository<Expediente> expedienteRepository, IRepository<Cita> citaRepository)
        {
            _expedienteRepository = expedienteRepository;
            _citaRepository = citaRepository;
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

        public List<Cita> ObtenerCitasPendientesDeExpediente()
        {
            // Citas que ya pasaron su fecha, siguen "Agendada" (no canceladas),
            // y todavía no tienen un expediente registrado
            var idsConExpediente = _expedienteRepository.Consultar()
                .Where(e => e.CitaId != null)
                .Select(e => e.CitaId!.Value)
                .ToList();

            return _citaRepository.Consultar()
                .Include(c => c.Paciente)
                .Include(c => c.Servicio)
                .Include(c => c.BloqueAgenda)
                .Where(c => c.Estado == EstadoCita.Agendada
                         && c.BloqueAgenda.Fecha <= DateTime.Today
                         && !idsConExpediente.Contains(c.CitaId))
                .OrderBy(c => c.BloqueAgenda.Fecha)
                .ToList();
        }

        public (bool exito, string mensaje) Crear(int citaId, int registradoPorUsuarioId, string diagnostico, string tratamiento, string? observaciones)
        {
            if (string.IsNullOrWhiteSpace(diagnostico) || string.IsNullOrWhiteSpace(tratamiento))
                return (false, "El diagnóstico y el tratamiento son obligatorios.");

            var cita = _citaRepository.ObtenerPorId(citaId);
            if (cita == null)
                return (false, "La cita no existe.");

            if (cita.Estado != EstadoCita.Agendada)
                return (false, "Solo se puede registrar un expediente para una cita Agendada.");

            var expediente = new Expediente
            {
                PacienteId = cita.PacienteId,
                CitaId = cita.CitaId,
                Diagnostico = diagnostico.Trim(),
                Tratamiento = tratamiento.Trim(),
                Observaciones = string.IsNullOrWhiteSpace(observaciones) ? null : observaciones.Trim(),
                RegistradoPorUsuarioId = registradoPorUsuarioId,
                FechaRegistro = DateTime.Now
            };

            // El registro del expediente es lo que marca la cita como Completada
            cita.Estado = EstadoCita.Completada;

            _expedienteRepository.Agregar(expediente);
            _expedienteRepository.GuardarCambios();

            return (true, "Expediente registrado correctamente. La cita quedó marcada como Completada.");
        }
    }
}