using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AuraDental.Data.Entities;

namespace AuraDental.Services
{
    public interface IAgendaService
    {
        List<BloqueAgenda> ObtenerTodos();
        List<BloqueAgenda> ObtenerPorFecha(DateTime fecha);
        BloqueAgenda? ObtenerPorId(int id);
        (bool exito, string mensaje) Crear(BloqueAgenda bloque);
        (bool exito, string mensaje) Actualizar(BloqueAgenda bloque);
        void Eliminar(int id);
        bool ExisteSolapamiento(DateTime fecha, TimeSpan horaInicio, TimeSpan horaFin, int? idExcluir = null);
    }
}