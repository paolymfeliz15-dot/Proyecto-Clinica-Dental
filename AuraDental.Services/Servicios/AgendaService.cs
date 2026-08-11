using AuraDental.Dominio.Entidades;
using AuraDental.Dominio.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AuraDental.Aplicacion
{
    public class AgendaService : IAgendaService
    {
        private readonly IRepository<BloqueAgenda> _agendaRepository;
        private readonly IRepository<Servicio> _servicioRepository;

        public AgendaService(IRepository<BloqueAgenda> agendaRepository, IRepository<Servicio> servicioRepository)
        {
            _agendaRepository = agendaRepository;
            _servicioRepository = servicioRepository;
        }

        public List<BloqueAgenda> ObtenerTodos()
        {
            return _agendaRepository.Consultar()
                .Include(b => b.Usuario)
                .OrderBy(b => b.Fecha).ThenBy(b => b.HoraInicio)
                .ToList();
        }

        public List<BloqueAgenda> ObtenerPorFecha(DateTime fecha)
        {
            return _agendaRepository.Consultar()
                .Include(b => b.Usuario)
                .Where(b => b.Fecha.Date == fecha.Date)
                .OrderBy(b => b.HoraInicio)
                .ToList();
        }

        public BloqueAgenda? ObtenerPorId(int id)
        {
            return _agendaRepository.Consultar()
                .Include(b => b.Usuario)
                .FirstOrDefault(b => b.BloqueAgendaId == id);
        }

        public List<BloqueAgenda> ObtenerDisponiblesPorServicio(int servicioId)
        {
            var servicio = _servicioRepository.ObtenerPorId(servicioId);
            if (servicio == null) return new List<BloqueAgenda>();

            // Se usa AsEnumerable() para permitir que el cálculo de TotalMinutes se ejecute en memoria,
            // ya que EF Core no puede traducir directamente esa operación a SQL.
            return _agendaRepository.Consultar()
                .Include(b => b.Usuario)
                .Where(b => b.Disponible && b.Fecha >= DateTime.Today)
                .AsEnumerable()
                .Where(b => (b.HoraFin - b.HoraInicio).TotalMinutes >= servicio.DuracionMinutos)
                .OrderBy(b => b.Fecha).ThenBy(b => b.HoraInicio)
                .ToList();
        }

        public bool ExisteSolapamiento(DateTime fecha, TimeSpan horaInicio, TimeSpan horaFin, int? idExcluir = null)
        {
            return _agendaRepository.Consultar()
                .Any(b => b.Fecha.Date == fecha.Date
                       && b.BloqueAgendaId != idExcluir
                       && horaInicio < b.HoraFin
                       && horaFin > b.HoraInicio);
        }

        public (bool exito, string mensaje) Crear(BloqueAgenda bloque)
        {
            if (bloque.HoraInicio >= bloque.HoraFin)
                return (false, "La hora de inicio debe ser anterior a la hora de fin.");

            if (ExisteSolapamiento(bloque.Fecha, bloque.HoraInicio, bloque.HoraFin))
                return (false, "Ya existe un bloque de agenda que se solapa con ese horario.");

            bloque.Disponible = true;
            _agendaRepository.Agregar(bloque);
            _agendaRepository.GuardarCambios();

            return (true, "Bloque creado correctamente.");
        }

        public (bool exito, string mensaje) Actualizar(BloqueAgenda bloque)
        {
            var existente = _agendaRepository.ObtenerPorId(bloque.BloqueAgendaId);
            if (existente == null)
                return (false, "El bloque no existe.");

            if (bloque.HoraInicio >= bloque.HoraFin)
                return (false, "La hora de inicio debe ser anterior a la hora de fin.");

            if (ExisteSolapamiento(bloque.Fecha, bloque.HoraInicio, bloque.HoraFin, bloque.BloqueAgendaId))
                return (false, "Ya existe otro bloque de agenda que se solapa con ese horario.");

            existente.Fecha = bloque.Fecha;
            existente.HoraInicio = bloque.HoraInicio;
            existente.HoraFin = bloque.HoraFin;
            existente.Disponible = bloque.Disponible;

            _agendaRepository.GuardarCambios();
            return (true, "Bloque actualizado correctamente.");
        }

        public void Eliminar(int id)
        {
            var bloque = _agendaRepository.ObtenerPorId(id);
            if (bloque == null) return;

            _agendaRepository.Eliminar(bloque);
            _agendaRepository.GuardarCambios();
        }
    }
}