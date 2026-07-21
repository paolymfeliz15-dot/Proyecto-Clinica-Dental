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
    public class CitaService : ICitaService
    {
        private readonly AuraDentalDbContext _context;

        public CitaService(AuraDentalDbContext context)
        {
            _context = context;
        }

        public List<Cita> ObtenerPorPaciente(int pacienteId)
        {
            return _context.Citas
                .Include(c => c.Servicio)
                .Include(c => c.BloqueAgenda)
                .Where(c => c.PacienteId == pacienteId)
                .OrderByDescending(c => c.BloqueAgenda.Fecha)
                .ToList();
        }

        public Cita? ObtenerPorId(int id)
        {
            return _context.Citas
                .Include(c => c.Servicio)
                .Include(c => c.BloqueAgenda)
                .Include(c => c.Paciente)
                .FirstOrDefault(c => c.CitaId == id);
        }

        public (bool exito, string mensaje) Cancelar(int citaId, int pacienteId)
        {
            var cita = _context.Citas
                .Include(c => c.BloqueAgenda)
                .FirstOrDefault(c => c.CitaId == citaId);

            if (cita == null)
                return (false, "La cita no existe.");

            // Seguridad: un paciente solo puede cancelar SUS propias citas
            if (cita.PacienteId != pacienteId)
                return (false, "No tienes permiso para cancelar esta cita.");

            if (cita.Estado == "Cancelada")
                return (false, "Esta cita ya estaba cancelada.");

            if (cita.BloqueAgenda.Fecha < DateTime.Today)
                return (false, "No se puede cancelar una cita que ya pasó.");

            cita.Estado = "Cancelada";
            cita.BloqueAgenda.Disponible = true; // libera el horario para que otro paciente lo use

            _context.SaveChanges();

            return (true, "Cita cancelada correctamente.");
        }
    }
}