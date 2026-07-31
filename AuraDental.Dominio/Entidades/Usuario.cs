using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuraDental.Dominio.Entidades
{
    public class Usuario
    {
        public int UsuarioId { get; set; }
        public string NombreCompleto { get; set; } = string.Empty; // Nombres
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        public int RolId { get; set; }
        public Rol Rol { get; set; } = null!;

        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // ===== Campos ampliados de perfil (HU-21) =====
        public string? Apellidos { get; set; }
        public string? Telefono { get; set; }
        public string? Cedula { get; set; }
        public string? Direccion { get; set; }
        public string? Pais { get; set; }
        public string? EstadoProvincia { get; set; } // viene de la API, ya no de la tabla local
        public string? Ciudad { get; set; }
        public string? Sector { get; set; }
        public string? FotoPerfilUrl { get; set; }

        public bool EmailVerificado { get; set; } = false;
        public string? TokenVerificacion { get; set; }
        public DateTime? TokenExpiracion { get; set; }
    }
}