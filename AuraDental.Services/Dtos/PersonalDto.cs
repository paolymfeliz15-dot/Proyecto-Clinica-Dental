using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuraDental.Aplicacion.Dtos
{
    public class PersonalDto
    {
        // UsuarioId solo se usa al editar; al crear queda en 0 y se ignora
        public int UsuarioId { get; set; }

        public string NombreCompleto { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Cedula { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Pais { get; set; } = string.Empty;
        public string EstadoProvincia { get; set; } = string.Empty;
        public string Ciudad { get; set; } = string.Empty;
        public string Sector { get; set; } = string.Empty;

        // RolId SÍ se incluye aquí a propósito: este formulario es exclusivo
        // del Administrador (protegido con [SessionAuthorize(RolRequerido = "Administrador")]),
        // así que elegir el rol en este contexto es legítimo, a diferencia del registro público.
        public int RolId { get; set; }

        // Solo se usa al crear (viene vacío al editar, y el servicio lo ignora en ese caso)
        public string? Password { get; set; }
    }
}