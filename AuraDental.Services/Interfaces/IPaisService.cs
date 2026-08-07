using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuraDental.Aplicacion
{
    public interface IPaisService
    {
        Task<List<string>> ObtenerPaisesAsync();
        Task<List<string>> ObtenerEstadosAsync(string pais);
        Task<List<string>> ObtenerCiudadesAsync(string pais, string estado);
    }
}