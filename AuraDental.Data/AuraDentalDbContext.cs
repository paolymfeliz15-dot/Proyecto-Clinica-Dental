using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AuraDental.Dominio.Entidades;

namespace AuraDental.Infraestructura
{
    public class AuraDentalDbContext : DbContext
    {
        public AuraDentalDbContext(DbContextOptions<AuraDentalDbContext> options)
            : base(options)
        {
        }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Servicio> Servicios { get; set; }
        public DbSet<BloqueAgenda> BloquesAgenda { get; set; }
        public DbSet<Cita> Citas { get; set; }
        public DbSet<Expediente> Expedientes { get; set; }
        public DbSet<Resena> Resenas { get; set; }
        public DbSet<Sugerencia> Sugerencias { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Un email no se puede repetir entre usuarios
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
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

            modelBuilder.Entity<BloqueAgenda>()
                .HasOne(b => b.Usuario)
                .WithMany()
                .HasForeignKey(b => b.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Cita>()
                .HasOne(c => c.Paciente)
                .WithMany()
                .HasForeignKey(c => c.PacienteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Cita>()
                .HasOne(c => c.Servicio)
                .WithMany()
                .HasForeignKey(c => c.ServicioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Cita>()
                .HasOne(c => c.BloqueAgenda)
                .WithMany()
                .HasForeignKey(c => c.BloqueAgendaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Cedula)
                .IsUnique()
                .HasFilter("[Cedula] IS NOT NULL"); // permite múltiples NULL (personal sin cédula registrada)

            modelBuilder.Entity<Expediente>()
                .HasOne(e => e.Paciente)
                .WithMany()
                .HasForeignKey(e => e.PacienteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Expediente>()
                .HasOne(e => e.Cita)
                .WithMany()
                .HasForeignKey(e => e.CitaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Expediente>()
                .HasOne(e => e.RegistradoPor)
                .WithMany()
                .HasForeignKey(e => e.RegistradoPorUsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Resena>()
                .HasOne(r => r.Paciente)
                .WithMany()
                .HasForeignKey(r => r.PacienteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Sugerencia>()
                .HasOne(s => s.Paciente)
                .WithMany()
                .HasForeignKey(s => s.PacienteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Cita>()
                .Property(c => c.Estado)
                .HasConversion<string>();

            modelBuilder.Entity<Usuario>().HasData(
                new Usuario
                {
                    UsuarioId = 1,
                    NombreCompleto = "Administrador",
                    Email = "Admin",
                    PasswordHash = "$2b$11$d5.lXPkCEuYNABoynEZpQ.MR6bzSzsgCJdkHRLWlT51wrH/wiWM5W",
                    RolId = 1,
                    Activo = true,
                    EmailVerificado = true,
                    FechaCreacion = new DateTime(2026, 7, 11)
                }
            );
        }
    }
}