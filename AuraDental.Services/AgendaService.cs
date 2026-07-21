using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuraDental.Data;
using AuraDental.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuraDental.Services
{
    public class AgendaService : IAgendaService
    {
        private readonly AuraDentalDbContext _context;

        public AgendaService(AuraDentalDbContext context)
        {
            _context = context;
        }

        public List<BloqueAgenda> ObtenerTodos()
        {
            return _context.BloquesAgenda
                .Include(b => b.Usuario)
                .OrderBy(b => b.Fecha)
                .ThenBy(b => b.HoraInicio)
                .ToList();
        }

        public List<BloqueAgenda> ObtenerPorFecha(DateTime fecha)
        {
            return _context.BloquesAgenda
                .Include(b => b.Usuario)
                .Where(b => b.Fecha.Date == fecha.Date)
                .OrderBy(b => b.HoraInicio)
                .ToList();
        }

        public List<BloqueAgenda> ObtenerDisponiblesPorServicio(int servicioId)
        {
            var servicio = _context.Servicios.Find(servicioId);
            if (servicio == null) return new List<BloqueAgenda>();

            // Solo bloques disponibles, futuros, y con duración suficiente para el servicio elegido
            return _context.BloquesAgenda
                .Include(b => b.Usuario)
                .Where(b => b.Disponible
                         && b.Fecha >= DateTime.Today
                         && (b.HoraFin - b.HoraInicio).TotalMinutes >= servicio.DuracionMinutos)
                .OrderBy(b => b.Fecha)
                .ThenBy(b => b.HoraInicio)
                .ToList();
        }

        public BloqueAgenda? ObtenerPorId(int id)
        {
            return _context.BloquesAgenda
                .Include(b => b.Usuario)
                .FirstOrDefault(b => b.BloqueAgendaId == id);
        }

        // Validación de solapamiento: dos bloques se solapan si uno empieza
        // antes de que el otro termine, Y termina después de que el otro empieza.
        public bool ExisteSolapamiento(DateTime fecha, TimeSpan horaInicio, TimeSpan horaFin, int? idExcluir = null)
        {
            return _context.BloquesAgenda
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
            _context.BloquesAgenda.Add(bloque);
            _context.SaveChanges();

            return (true, "Bloque creado correctamente.");
        }

        public (bool exito, string mensaje) Actualizar(BloqueAgenda bloque)
        {
            var existente = _context.BloquesAgenda.Find(bloque.BloqueAgendaId);
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

            _context.SaveChanges();
            return (true, "Bloque actualizado correctamente.");
        }

        public void Eliminar(int id)
        {
            var bloque = _context.BloquesAgenda.Find(id);
            if (bloque == null) return;

            _context.BloquesAgenda.Remove(bloque);
            _context.SaveChanges();
        }
    }
}