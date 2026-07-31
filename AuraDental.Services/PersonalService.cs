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

        public List<Usuario> ObtenerTodos()
        {
            return _usuarioRepository.Consultar()
                .Include(u => u.Rol)
                .Where(u => u.Rol.Nombre == "Administrador" || u.Rol.Nombre == "Asistente")
                .OrderBy(u => u.NombreCompleto)
                .ToList();
        }

        public Usuario? ObtenerPorId(int id)
        {
            return _usuarioRepository.Consultar()
                .Include(u => u.Rol)
                .FirstOrDefault(u => u.UsuarioId == id);
        }

        public bool ExisteEmail(string email, int? idExcluir = null)
        {
            return _usuarioRepository.Consultar()
                .Any(u => u.Email == email && u.UsuarioId != idExcluir);
        }

        public void Crear(Usuario usuario, string password)
        {
            usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            usuario.Activo = true;
            usuario.FechaCreacion = DateTime.Now;

            _usuarioRepository.Agregar(usuario);
            _usuarioRepository.GuardarCambios();
        }

        public void Actualizar(Usuario usuario)
        {
            var existente = _usuarioRepository.ObtenerPorId(usuario.UsuarioId);
            if (existente == null) return;

            existente.NombreCompleto = usuario.NombreCompleto;
            existente.Email = usuario.Email;
            existente.RolId = usuario.RolId;
            existente.Apellidos = usuario.Apellidos;
            existente.Telefono = usuario.Telefono;
            existente.Cedula = usuario.Cedula;
            existente.Direccion = usuario.Direccion;
            existente.Pais = usuario.Pais;
            existente.EstadoProvincia = usuario.EstadoProvincia;
            existente.Ciudad = usuario.Ciudad;
            existente.Sector = usuario.Sector;

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