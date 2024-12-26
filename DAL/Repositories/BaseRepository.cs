using DAL.Data;
using DAL.Interfaces;
using DAL.Parameters;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        private readonly AppWriteDbContext _writeContext;
        private readonly AppReadDbContext _readContext;

        public BaseRepository(AppWriteDbContext writeContext, AppReadDbContext readContext)
        {
            _writeContext = writeContext;
            _readContext = readContext;
        }

        public async Task<TEntity> CreateAsync(TEntity createdEntity)
        {
            await _writeContext.Set<TEntity>().AddAsync(createdEntity);
            return createdEntity;
        }

        public async Task<IEnumerable<TEntity>> DeleteAllAsync()
        {
            var entities = await _writeContext.Set<TEntity>().ToListAsync();
            _writeContext.Set<TEntity>().RemoveRange(entities);
            await _writeContext.SaveChangesAsync();
            return entities;
        }

        public async Task<TEntity> DeleteByIdAsync(int id)
        {
            var entity = await _writeContext.Set<TEntity>().FindAsync(id);
            if (entity == null)
                return null;

            _writeContext.Set<TEntity>().Remove(entity);
            await _writeContext.SaveChangesAsync();
            return entity;
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(PaginationParameters pagination)
        {
            var entities = _readContext.Set<TEntity>().AsQueryable();
            return await entities.Skip((pagination.Page - 1) * pagination.PageSize).Take(pagination.PageSize).ToListAsync();
        }

        public async Task<TEntity> GetByIdAsync(int id)
        {
            var entity = await _readContext.Set<TEntity>().FindAsync(id);
            if (entity == null)
                return null;

            return entity;
        }

        public async Task<TEntity> UpdateAsync(int id, TEntity updatedEntity)
        {
            var entity = await _writeContext.Set<TEntity>().FindAsync(id);
            if (entity == null)
                return null;

            _writeContext.Entry(entity).CurrentValues.SetValues(updatedEntity);
            return updatedEntity;
        }
    }
}
