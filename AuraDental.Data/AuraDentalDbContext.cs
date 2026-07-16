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

        public DbSet<Servicio> Servicios { get; set; }

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

            // Usuario Administrador por defecto (contraseña: Admin123, ya hasheada con BCrypt).
            // Se crea automáticamente al aplicar las migraciones, no se puede registrar
            // un Administrador desde el formulario público.

            modelBuilder.Entity<Servicio>()
    .HasIndex(s => s.Nombre)
    .IsUnique();

            modelBuilder.Entity<Servicio>()
                .Property(s => s.Precio)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<Usuario>().HasData(
                new Usuario
                {
                    UsuarioId = 1,
                    NombreCompleto = "Administrador",
                    Email = "Admin",
                    PasswordHash = "$2b$11$d5.lXPkCEuYNABoynEZpQ.MR6bzSzsgCJdkHRLWlT51wrH/wiWM5W",
                    RolId = 1,
                    Activo = true,
                    FechaCreacion = new DateTime(2026, 7, 11)
                }
            );
        }
    }
}