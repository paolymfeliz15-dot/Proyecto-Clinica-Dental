using AuraDental.Dominio.Entidades;
using AuraDental.Dominio.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AuraDental.Aplicacion
{
    public class ResenaService : IResenaService
    {
        private readonly IRepository<Resena> _resenaRepository;

        public ResenaService(IRepository<Resena> resenaRepository)
        {
            _resenaRepository = resenaRepository;
        }

        public List<Resena> ObtenerTodas()
        {
            return _resenaRepository.Consultar()
                .Include(r => r.Paciente)
                .OrderByDescending(r => r.FechaCreacion)
                .ToList();
        }

        public (bool exito, string mensaje) Crear(int pacienteId, int calificacion, string comentario)
        {
            if (calificacion < 1 || calificacion > 5)
                return (false, "La calificación debe estar entre 1 y 5 estrellas.");

            if (string.IsNullOrWhiteSpace(comentario))
                return (false, "El comentario no puede estar vacío.");

            var resena = new Resena
            {
                PacienteId = pacienteId,
                Calificacion = calificacion,
                Comentario = comentario.Trim(),
                FechaCreacion = DateTime.Now
            };

            _resenaRepository.Agregar(resena);
            _resenaRepository.GuardarCambios();

            return (true, "¡Gracias por tu reseña!");
        }
    }
}