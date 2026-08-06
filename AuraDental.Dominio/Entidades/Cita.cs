using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuraDental.Dominio.Entidades
{
    public class Cita
    {
        public int CitaId { get; set; }

        public int PacienteId { get; set; }
        public Usuario Paciente { get; set; } = null!;

        public int ServicioId { get; set; }
        public Servicio Servicio { get; set; } = null!;

        public int BloqueAgendaId { get; set; }
        public BloqueAgenda BloqueAgenda { get; set; } = null!;

        public EstadoCita Estado { get; set; } = EstadoCita.Agendada;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;
    }
}