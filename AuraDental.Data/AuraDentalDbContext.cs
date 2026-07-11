using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AuraDental.Data
{
    public class AuraDentalDbContext : DbContext
    {
        public AuraDentalDbContext(DbContextOptions<AuraDentalDbContext> options)
            : base(options)
        {
        }

        // Aquí irán los DbSet<> de cada entidad (Paciente, Cita, etc.)
        // según vayan avanzando las Historias de Usuario del Sprint 1

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Aquí irán las configuraciones Fluent API de cada entidad
        }
    }
}
