using DAL.Data;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        private readonly AppDbContext _context;

        public BaseRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TEntity> CreateAsync(TEntity createdEntity)
        {
            await _context.Set<TEntity>().AddAsync(createdEntity);
            return createdEntity;
        }

        public async Task<IEnumerable<TEntity>> DeleteAllAsync()
        {
            var entities = await _context.Set<TEntity>().ToListAsync();
            _context.Set<TEntity>().RemoveRange(entities);
            await _context.SaveChangesAsync();
            return entities;
        }

        public async Task<TEntity> DeleteByIdAsync(int id)
        {
            var entity = await _context.Set<TEntity>().FindAsync(id);
            if (entity == null)
                return null;

            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _context.Set<TEntity>().ToListAsync();
        }

        public async Task<TEntity> GetByIdAsync(int id)
        {
            var entity = await _context.Set<TEntity>().FindAsync(id);
            if (entity == null)
                return null;

            return entity;
        }

        public async Task<TEntity> UpdateAsync(int id, TEntity updatedEntity)
        {
            var entity = await _context.Set<TEntity>().FindAsync(id);
            if (entity == null)
                return null;

            _context.Entry(entity).CurrentValues.SetValues(updatedEntity);
            return updatedEntity;
        }
    }
}
