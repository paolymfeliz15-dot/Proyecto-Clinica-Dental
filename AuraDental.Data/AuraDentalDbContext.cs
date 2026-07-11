using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AuraDental.Data.Entities;

namespace AuraDental.Data
{
    public class AuraDentalDbContext : DbContext
    {
        public AuraDentalDbContext(DbContextOptions<AuraDentalDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Provincia> Provincias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Un email no se puede repetir entre usuarios
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Un nombre de provincia no se puede repetir
            modelBuilder.Entity<Provincia>()
                .HasIndex(p => p.Nombre)
                .IsUnique();

            // Datos semilla: los 3 roles del sistema, ya creados desde el inicio
            modelBuilder.Entity<Rol>().HasData(
                new Rol { RolId = 1, Nombre = "Administrador" },
                new Rol { RolId = 2, Nombre = "Paciente" },
                new Rol { RolId = 3, Nombre = "Asistente" }
            );
        }
    }
}