using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuraDental.Dominio.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AuraDental.Infraestructura.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly AuraDentalDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(AuraDentalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public T? ObtenerPorId(int id) => _dbSet.Find(id);

        public IQueryable<T> Consultar() => _dbSet.AsQueryable();

        public void Agregar(T entidad) => _dbSet.Add(entidad);

        public void Actualizar(T entidad) => _dbSet.Update(entidad);

        public void Eliminar(T entidad) => _dbSet.Remove(entidad);

        public void GuardarCambios() => _context.SaveChanges();
    }
}