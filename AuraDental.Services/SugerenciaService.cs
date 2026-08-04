using AuraDental.Dominio.Entidades;
using AuraDental.Dominio.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AuraDental.Aplicacion
{
    public class SugerenciaService : ISugerenciaService
    {
        private readonly IRepository<Sugerencia> _sugerenciaRepository;

        public SugerenciaService(IRepository<Sugerencia> sugerenciaRepository)
        {
            _sugerenciaRepository = sugerenciaRepository;
        }

        public (bool exito, string mensaje) Crear(int pacienteId, string mensaje)
        {
            if (string.IsNullOrWhiteSpace(mensaje))
                return (false, "El mensaje no puede estar vacío.");

            var sugerencia = new Sugerencia
            {
                PacienteId = pacienteId,
                Mensaje = mensaje.Trim(),
                Leida = false,
                FechaCreacion = DateTime.Now
            };

            _sugerenciaRepository.Agregar(sugerencia);
            _sugerenciaRepository.GuardarCambios();

            return (true, "¡Gracias por tu sugerencia! La revisaremos pronto.");
        }

        public List<Sugerencia> ObtenerTodas()
        {
            return _sugerenciaRepository.Consultar()
                .Include(s => s.Paciente)
                .OrderByDescending(s => s.FechaCreacion)
                .ToList();
        }

        public int ContarNoLeidas()
        {
            return _sugerenciaRepository.Consultar().Count(s => !s.Leida);
        }

        public void MarcarComoLeida(int sugerenciaId)
        {
            var sugerencia = _sugerenciaRepository.ObtenerPorId(sugerenciaId);
            if (sugerencia == null) return;

            sugerencia.Leida = true;
            _sugerenciaRepository.GuardarCambios();
        }
    }
}
