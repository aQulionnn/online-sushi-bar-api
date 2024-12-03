using DAL.Parameters;

namespace DAL.Interfaces
{
    public interface IBaseRepository<TEntity>
    {
        Task<TEntity> CreateAsync(TEntity createdEntity);
        Task<IEnumerable<TEntity>> GetAllAsync(PaginationParameters pagination);
        Task<TEntity> GetByIdAsync(int id);
        Task<TEntity> UpdateAsync(int id, TEntity updatedEntity);
        Task<IEnumerable<TEntity>> DeleteAllAsync();
        Task<TEntity> DeleteByIdAsync(int id);
    }
}
