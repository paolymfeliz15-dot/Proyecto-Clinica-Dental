using AuraDental.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace AuraDental.Services
{
    public interface IAgendaService
    {
        List<BloqueAgenda> ObtenerTodos();
        List<BloqueAgenda> ObtenerPorFecha(DateTime fecha);

        List<BloqueAgenda> ObtenerDisponiblesPorServicio(int servicioId);
        BloqueAgenda? ObtenerPorId(int id);
        (bool exito, string mensaje) Crear(BloqueAgenda bloque);
        (bool exito, string mensaje) Actualizar(BloqueAgenda bloque);
        void Eliminar(int id);
        bool ExisteSolapamiento(DateTime fecha, TimeSpan horaInicio, TimeSpan horaFin, int? idExcluir = null);
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
    }