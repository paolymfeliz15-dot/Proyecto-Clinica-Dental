using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuraDental.Data.Entities
{
    public class Provincia
    {
        public int ProvinciaId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public bool Activa { get; set; } = true;

        // Relación futura con Paciente, mapeada conceptualmente
        // pero sin implementar todavía (llega en Sprint 3)
    }
}