using AuraDental.Dominio.Entidades;
using AuraDental.Dominio.Interfaces;

namespace AuraDental.Aplicacion
{
    public class ProvinciaService : IProvinciaService
    {
        private readonly IRepository<Provincia> _provinciaRepository;

        public ProvinciaService(IRepository<Provincia> provinciaRepository)
        {
            _provinciaRepository = provinciaRepository;
        }

        public List<Provincia> ObtenerTodos()
        {
            return _provinciaRepository.Consultar().OrderBy(p => p.Nombre).ToList();
        }

        public Provincia? ObtenerPorId(int id) => _provinciaRepository.ObtenerPorId(id);

        public bool ExisteNombre(string nombre, int? idExcluir = null)
        {
            return _provinciaRepository.Consultar()
                .Any(p => p.Nombre == nombre && p.ProvinciaId != idExcluir);
        }

        public void Crear(Provincia provincia)
        {
            provincia.Activa = true;
            _provinciaRepository.Agregar(provincia);
            _provinciaRepository.GuardarCambios();
        }

        public void Actualizar(Provincia provincia)
        {
            var existente = _provinciaRepository.ObtenerPorId(provincia.ProvinciaId);
            if (existente == null) return;

            existente.Nombre = provincia.Nombre;
            _provinciaRepository.GuardarCambios();
        }

        public void CambiarEstado(int id, bool activa)
        {
            var provincia = _provinciaRepository.ObtenerPorId(id);
            if (provincia == null) return;

            provincia.Activa = activa;
            _provinciaRepository.GuardarCambios();
        }
    }
}