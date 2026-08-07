using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuraDental.Aplicacion.Dtos
{
    public class UsuarioResumenDto
    {
        public int UsuarioId { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Cedula { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Pais { get; set; } = string.Empty;
        public string EstadoProvincia { get; set; } = string.Empty;
        public string Ciudad { get; set; } = string.Empty;
        public string Sector { get; set; } = string.Empty;
        public string NombreRol { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}