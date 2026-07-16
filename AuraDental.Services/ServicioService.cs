using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuraDental.Data;
using AuraDental.Data.Entities;

namespace AuraDental.Services
{
    public class ServicioService : IServicioService
    {
        private readonly AuraDentalDbContext _context;

        public ServicioService(AuraDentalDbContext context)
        {
            _context = context;
        }

        public List<Servicio> ObtenerTodos()
        {
            return _context.Servicios
                .OrderBy(s => s.Nombre)
                .ToList();
        }

        public Servicio? ObtenerPorId(int id)
        {
            return _context.Servicios.Find(id);
        }

        public bool ExisteNombre(string nombre, int? idExcluir = null)
        {
            return _context.Servicios
                .Any(s => s.Nombre == nombre && s.ServicioId != idExcluir);
        }

        public void Crear(Servicio servicio)
        {
            servicio.Activo = true;
            _context.Servicios.Add(servicio);
            _context.SaveChanges();
        }

        public void Actualizar(Servicio servicio)
        {
            var existente = _context.Servicios.Find(servicio.ServicioId);
            if (existente == null) return;

            existente.Nombre = servicio.Nombre;
            existente.Descripcion = servicio.Descripcion;
            existente.DuracionMinutos = servicio.DuracionMinutos;
            existente.Precio = servicio.Precio;

            _context.SaveChanges();
        }

        public void CambiarEstado(int id, bool activo)
        {
            var servicio = _context.Servicios.Find(id);
            if (servicio == null) return;

            servicio.Activo = activo;
            _context.SaveChanges();
        }
    }
}
