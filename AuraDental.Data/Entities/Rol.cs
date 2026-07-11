
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuraDental.Data.Entities
{
    public class Rol
    {
        public int RolId { get; set; }
        public string Nombre { get; set; } = string.Empty; // Administrador, Paciente, Asistente

        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}