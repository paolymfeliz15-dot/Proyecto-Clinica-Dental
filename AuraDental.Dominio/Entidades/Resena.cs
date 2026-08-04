namespace AuraDental.Dominio.Entidades
{
    public class Resena
    {
        public int ResenaId { get; set; }

        public int PacienteId { get; set; }
        public Usuario Paciente { get; set; } = null!;

        public int Calificacion { get; set; } // 1 a 5
        public string Comentario { get; set; } = string.Empty;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;
    }
}