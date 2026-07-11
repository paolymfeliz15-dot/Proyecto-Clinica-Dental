using AuraDental.Data;
using AuraDental.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuraDental.Services
{
    public class PersonalService : IPersonalService
    {
        private readonly AuraDentalDbContext _context;

        public PersonalService(AuraDentalDbContext context)
        {
            _context = context;
        }

        public List<Usuario> ObtenerTodos()
        {
            // Solo Administradores y Asistentes son "Personal".
            // Los Pacientes se gestionan aparte (no en esta pantalla).
            return _context.Usuarios
                .Include(u => u.Rol)
                .Where(u => u.Rol.Nombre == "Administrador" || u.Rol.Nombre == "Asistente")
                .OrderBy(u => u.NombreCompleto)
                .ToList();
        }

        public Usuario? ObtenerPorId(int id)
        {
            return _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefault(u => u.UsuarioId == id);
        }

        public bool ExisteEmail(string email, int? idExcluir = null)
        {
            return _context.Usuarios
                .Any(u => u.Email == email && u.UsuarioId != idExcluir);
        }

        public void Crear(Usuario usuario, string password)
        {
            usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            usuario.Activo = true;
            usuario.FechaCreacion = DateTime.Now;

            _context.Usuarios.Add(usuario);
            _context.SaveChanges();
        }

        public void Actualizar(Usuario usuario)
        {
            var existente = _context.Usuarios.Find(usuario.UsuarioId);
            if (existente == null) return;

            existente.NombreCompleto = usuario.NombreCompleto;
            existente.Email = usuario.Email;
            existente.RolId = usuario.RolId;

            _context.SaveChanges();
        }

        public void CambiarEstado(int id, bool activo)
        {
            var usuario = _context.Usuarios.Find(id);
            if (usuario == null) return;

            usuario.Activo = activo;
            _context.SaveChanges();
        }
    }
}