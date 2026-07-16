using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuraDental.Data.Entities
{
    public class Servicio
    {
        public int ServicioId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public int DuracionMinutos { get; set; }
        public decimal Precio { get; set; }

        // Baja lógica, igual que Usuario y Provincia en el Sprint 1
        public bool Activo { get; set; } = true;
    }
}
