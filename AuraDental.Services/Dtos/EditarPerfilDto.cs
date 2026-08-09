namespace AuraDental.Aplicacion.Dtos
{
    // Mismos campos que el registro/Personal — cualquier usuario autenticado
    // (Administrador, Asistente o Paciente) puede editar toda su información aquí.
    // No incluye RolId ni Password: el rol nunca se autoedita, y la contraseña
    // sigue teniendo su propio flujo separado (HU-11), por seguridad.
    public class EditarPerfilDto
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
    }
}