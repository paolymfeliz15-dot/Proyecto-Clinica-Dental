using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuraDental.Dominio.Interfaces
{
    public interface IRepository<T> where T : class
    {
        T? ObtenerPorId(int id);

        // Devuelve IQueryable para permitir consultas LINQ (Where, Include, OrderBy, etc.)
        // desde la capa de Aplicación, sin que esa capa necesite conocer Entity Framework directamente.
        IQueryable<T> Consultar();

        void Agregar(T entidad);
        void Actualizar(T entidad);
        void Eliminar(T entidad);
        void GuardarCambios();
    }
}