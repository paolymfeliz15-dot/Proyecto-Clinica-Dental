using AuraDental.Aplicacion.Dtos;
using AuraDental.Aplicacion.Mappers;
using AuraDental.Dominio.Entidades;
using AuraDental.Dominio.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AuraDental.Aplicacion
{
    public class PersonalService : IPersonalService
    {
        private readonly IRepository<Usuario> _usuarioRepository;

        public PersonalService(IRepository<Usuario> usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public List<UsuarioResumenDto> ObtenerTodos()
        {
            var usuarios = _usuarioRepository.Consultar()
                .Include(u => u.Rol)
                .Where(u => u.Rol.Nombre == "Administrador" || u.Rol.Nombre == "Asistente")
                .OrderBy(u => u.NombreCompleto)
                .ToList();

            return UsuarioMapper.AResumenLista(usuarios);
        }

        public UsuarioResumenDto? ObtenerPorId(int id)
        {
            var usuario = _usuarioRepository.Consultar()
                .Include(u => u.Rol)
                .FirstOrDefault(u => u.UsuarioId == id);

            return usuario == null ? null : UsuarioMapper.AResumen(usuario);
        }

        public bool ExisteEmail(string email, int? idExcluir = null)
        {
            return _usuarioRepository.Consultar()
                .Any(u => u.Email == email && u.UsuarioId != idExcluir);
        }

        public void Crear(PersonalDto datos)
        {
            var usuario = UsuarioMapper.APersonalNuevo(datos);
            usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(datos.Password ?? string.Empty);
            usuario.Activo = true;
            usuario.FechaCreacion = DateTime.Now;

            _usuarioRepository.Agregar(usuario);
            _usuarioRepository.GuardarCambios();
        }

        public void Actualizar(PersonalDto datos)
        {
            var existente = _usuarioRepository.ObtenerPorId(datos.UsuarioId);
            if (existente == null) return;

            UsuarioMapper.ActualizarDesdePersonalDto(existente, datos);
            _usuarioRepository.GuardarCambios();
        }

        public void CambiarEstado(int id, bool activo)
        {
            var usuario = _usuarioRepository.ObtenerPorId(id);
            if (usuario == null) return;

            usuario.Activo = activo;
            _usuarioRepository.GuardarCambios();
        }
    }
}