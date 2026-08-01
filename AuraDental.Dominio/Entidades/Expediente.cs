namespace AuraDental.Dominio.Entidades
{
    public class Expediente
    {
        public int ExpedienteId { get; set; }

        public int PacienteId { get; set; }
        public Usuario Paciente { get; set; } = null!;

        public int? CitaId { get; set; }
        public Cita? Cita { get; set; }

        public string Diagnostico { get; set; } = string.Empty;
        public string Tratamiento { get; set; } = string.Empty;
        public string? Observaciones { get; set; }

        // Quién lo registró (el Asistente) — se llena en HU-06
        public int RegistradoPorUsuarioId { get; set; }
        public Usuario RegistradoPor { get; set; } = null!;

        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}