using System.IO;
using AuraDental.Dominio.Entidades;
using AuraDental.Dominio.Interfaces;
using AuraDental.Dominio.ObjetosValor;

namespace AuraDental.Aplicacion
{
    public class ServicioService : IServicioService
    {
        private readonly IRepository<Servicio> _servicioRepository;

        public ServicioService(IRepository<Servicio> servicioRepository)
        {
            _servicioRepository = servicioRepository;
        }

        public List<Servicio> ObtenerTodos()
        {
            return _servicioRepository.Consultar()
                .OrderBy(s => s.Nombre)
                .ToList();
        }

        public Servicio? ObtenerPorId(int id) => _servicioRepository.ObtenerPorId(id);

        public bool ExisteNombre(string nombre, int? idExcluir = null)
        {
            return _servicioRepository.Consultar()
                .Any(s => s.Nombre == nombre && s.ServicioId != idExcluir);
        }

        public (bool exito, string mensaje) Crear(Servicio servicio)
        {
            var (dineroValido, mensajeDinero, dinero) = Dinero.Crear(servicio.Precio);
            if (!dineroValido)
                return (false, mensajeDinero);

            servicio.Precio = dinero!.Monto;
            servicio.Activo = true;
            _servicioRepository.Agregar(servicio);
            _servicioRepository.GuardarCambios();

            return (true, "Servicio creado correctamente.");
        }

        public void Actualizar(Servicio servicio)
        {
            var existente = _servicioRepository.ObtenerPorId(servicio.ServicioId);
            if (existente == null) return;

            existente.Nombre = servicio.Nombre;
            existente.Descripcion = servicio.Descripcion;
            existente.DuracionMinutos = servicio.DuracionMinutos;
            existente.Precio = servicio.Precio;

            _servicioRepository.GuardarCambios();
        }

        public void CambiarEstado(int id, bool activo)
        {
            var servicio = _servicioRepository.ObtenerPorId(id);
            if (servicio == null) return;

            servicio.Activo = activo;
            _servicioRepository.GuardarCambios();
        }

        public (bool exito, string mensaje) SubirImagen(int servicioId, byte[] contenidoArchivo, string extension)
        {
            var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            const long tamanoMaximoBytes = 4 * 1024 * 1024; // 4 MB

            var servicio = _servicioRepository.ObtenerPorId(servicioId);
            if (servicio == null)
                return (false, "Servicio no encontrado.");

            extension = extension.ToLowerInvariant();
            if (!extensionesPermitidas.Contains(extension))
                return (false, "Formato de imagen no permitido. Usa JPG, PNG o WEBP.");

            if (contenidoArchivo.Length > tamanoMaximoBytes)
                return (false, "La imagen no puede superar los 4 MB.");

            var nombreArchivo = $"{servicioId}_{Guid.NewGuid()}{extension}";
            var carpetaDestino = Path.Combine("wwwroot", "uploads", "servicios");
            Directory.CreateDirectory(carpetaDestino);

            var rutaFisica = Path.Combine(carpetaDestino, nombreArchivo);
            File.WriteAllBytes(rutaFisica, contenidoArchivo);

            if (!string.IsNullOrWhiteSpace(servicio.ImagenUrl))
            {
                var rutaAnterior = Path.Combine("wwwroot", servicio.ImagenUrl.TrimStart('/'));
                if (File.Exists(rutaAnterior))
                    File.Delete(rutaAnterior);
            }

            servicio.ImagenUrl = $"/uploads/servicios/{nombreArchivo}";
            _servicioRepository.GuardarCambios();

            return (true, "Imagen actualizada correctamente.");
        }
    }
}