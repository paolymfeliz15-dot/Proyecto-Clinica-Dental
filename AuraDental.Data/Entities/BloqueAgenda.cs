using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuraDental.Data.Entities
{
    public class BloqueAgenda
    {
        public int BloqueAgendaId { get; set; }

        public DateTime Fecha { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }

        public bool Disponible { get; set; } = true;

        // Quién creó/gestiona este bloque (el Asistente)
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;
    }
}