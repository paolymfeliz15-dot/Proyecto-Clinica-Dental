namespace AuraDental.Dominio.Entidades
{
    public class Sugerencia
    {
        public int SugerenciaId { get; set; }

        public int PacienteId { get; set; }
        public Usuario Paciente { get; set; } = null!;

        public string Mensaje { get; set; } = string.Empty;

        // Usado por HU-28 para la campana de notificaciones del Administrador
        public bool Leida { get; set; } = false;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;
    }
}