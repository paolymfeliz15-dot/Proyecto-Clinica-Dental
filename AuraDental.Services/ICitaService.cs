using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuraDental.Data.Entities;

namespace AuraDental.Services
{
    public interface ICitaService
    {
        List<Cita> ObtenerPorPaciente(int pacienteId);
        Cita? ObtenerPorId(int id);
        (bool exito, string mensaje) Cancelar(int citaId, int pacienteId);
    }
}