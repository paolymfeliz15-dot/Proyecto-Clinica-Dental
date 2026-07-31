using AuraDental.Dominio.Entidades;
using AuraDental.Dominio.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AuraDental.Aplicacion
{
    public class CitaService : ICitaService
    {
        private readonly IRepository<Cita> _citaRepository;
        private readonly IRepository<Servicio> _servicioRepository;
        private readonly IRepository<BloqueAgenda> _agendaRepository;

        public CitaService(IRepository<Cita> citaRepository, IRepository<Servicio> servicioRepository, IRepository<BloqueAgenda> agendaRepository)
        {
            _citaRepository = citaRepository;
            _servicioRepository = servicioRepository;
            _agendaRepository = agendaRepository;
        }

        public List<Cita> ObtenerPorPaciente(int pacienteId)
        {
            return _citaRepository.Consultar()
                .Include(c => c.Servicio)
                .Include(c => c.BloqueAgenda)
                .Where(c => c.PacienteId == pacienteId)
                .OrderByDescending(c => c.BloqueAgenda.Fecha)
                .ToList();
        }

        public Cita? ObtenerPorId(int id)
        {
            return _citaRepository.Consultar()
                .Include(c => c.Servicio)
                .Include(c => c.BloqueAgenda)
                .Include(c => c.Paciente)
                .FirstOrDefault(c => c.CitaId == id);
        }

        public (bool exito, string mensaje) Agendar(int pacienteId, int servicioId, int bloqueAgendaId)
        {
            var servicio = _servicioRepository.ObtenerPorId(servicioId);
            if (servicio == null || !servicio.Activo)
                return (false, "El servicio seleccionado no está disponible.");

            var bloque = _agendaRepository.ObtenerPorId(bloqueAgendaId);
            if (bloque == null)
                return (false, "El horario seleccionado no existe.");

            if (!bloque.Disponible)
                return (false, "Ese horario ya no está disponible. Por favor elige otro.");

            if (bloque.Fecha < DateTime.Today)
                return (false, "No se puede agendar en una fecha pasada.");

            if ((bloque.HoraFin - bloque.HoraInicio).TotalMinutes < servicio.DuracionMinutos)
                return (false, "Ese horario no tiene duración suficiente para el servicio seleccionado.");

            var cita = new Cita
            {
                PacienteId = pacienteId,
                ServicioId = servicioId,
                BloqueAgendaId = bloqueAgendaId,
                Estado = "Agendada",
                FechaCreacion = DateTime.Now
            };

            bloque.Disponible = false;

            _citaRepository.Agregar(cita);
            _citaRepository.GuardarCambios();

            return (true, "Cita agendada correctamente.");
        }

        public (bool exito, string mensaje) Cancelar(int citaId, int pacienteId)
        {
            var cita = _citaRepository.Consultar()
                .Include(c => c.BloqueAgenda)
                .FirstOrDefault(c => c.CitaId == citaId);

            if (cita == null)
                return (false, "La cita no existe.");

            if (cita.PacienteId != pacienteId)
                return (false, "No tienes permiso para cancelar esta cita.");

            if (cita.Estado == "Cancelada")
                return (false, "Esta cita ya estaba cancelada.");

            if (cita.BloqueAgenda.Fecha < DateTime.Today)
                return (false, "No se puede cancelar una cita que ya pasó.");

            cita.Estado = "Cancelada";
            cita.BloqueAgenda.Disponible = true;

            _citaRepository.GuardarCambios();

            return (true, "Cita cancelada correctamente.");
        }
    }
}