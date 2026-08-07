using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuraDental.Dominio.Entidades;

namespace AuraDental.Aplicacion
{
    public interface IServicioService
    {
        List<Servicio> ObtenerTodos();
        Servicio? ObtenerPorId(int id);
        void Crear(Servicio servicio);
        void Actualizar(Servicio servicio);
        void CambiarEstado(int id, bool activo);
        bool ExisteNombre(string nombre, int? idExcluir = null);
        (bool exito, string mensaje) SubirImagen(int servicioId, byte[] contenidoArchivo, string extension);
    }
}
