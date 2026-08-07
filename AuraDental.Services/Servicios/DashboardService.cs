using AuraDental.Aplicacion.Dtos;
using AuraDental.Dominio.Entidades;
using AuraDental.Dominio.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AuraDental.Aplicacion
{
    public class DashboardService : IDashboardService
    {
        private readonly IRepository<Cita> _citaRepository;

        public DashboardService(IRepository<Cita> citaRepository)
        {
            _citaRepository = citaRepository;
        }

        public EstadisticasDashboardDto ObtenerEstadisticas()
        {
            var citas = _citaRepository.Consultar()
                .Include(c => c.Servicio)
                .Include(c => c.BloqueAgenda)
                .ToList();

            var dto = new EstadisticasDashboardDto();

            // ===== Citas por semana (últimas 8 semanas) =====
            var hoy = DateTime.Today;
            for (int i = 7; i >= 0; i--)
            {
                var inicioSemana = hoy.AddDays(-7 * i - (int)hoy.DayOfWeek);
                var finSemana = inicioSemana.AddDays(6);

                var cantidad = citas.Count(c =>
                    c.BloqueAgenda.Fecha.Date >= inicioSemana && c.BloqueAgenda.Fecha.Date <= finSemana);

                dto.EtiquetasSemanas.Add(inicioSemana.ToString("dd/MM"));
                dto.CitasPorSemana.Add(cantidad);
            }

            // ===== Servicios más solicitados (Top 5) =====
            var topServicios = citas
                .GroupBy(c => c.Servicio.Nombre)
                .Select(g => new { Nombre = g.Key, Cantidad = g.Count() })
                .OrderByDescending(g => g.Cantidad)
                .Take(5)
                .ToList();

            dto.NombresServicios = topServicios.Select(s => s.Nombre).ToList();
            dto.CantidadPorServicio = topServicios.Select(s => s.Cantidad).ToList();

            // ===== Ingresos estimados (solo citas completadas) =====
            dto.TotalCitasCompletadas = citas.Count(c => c.Estado == EstadoCita.Completada);
            dto.IngresosEstimados = citas
                .Where(c => c.Estado == EstadoCita.Completada)
                .Sum(c => c.Servicio.Precio);

            return dto;
        }
    }
}