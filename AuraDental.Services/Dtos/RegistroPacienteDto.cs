using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuraDental.Aplicacion.Dtos
{
    // Contiene EXACTAMENTE los campos que un visitante puede enviar al registrarse.
    // Nunca incluye RolId, Activo, PasswordHash ni EmailVerificado — esos los decide
    // el servidor, nunca el formulario.
    public class RegistroPacienteDto
    {
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
        public string Password { get; set; } = string.Empty;
    }
}