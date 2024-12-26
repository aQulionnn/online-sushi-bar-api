using DAL.Data;
using DAL.Interfaces;

namespace DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppWriteDbContext _writeContext;
        private readonly AppReadDbContext _readContext;
        private IMenuItemRepository _menuItemRepo;

        public UnitOfWork(AppWriteDbContext writeContext, AppReadDbContext readContext)
        {
            _writeContext = writeContext;
            _readContext = readContext;
        }

        public IMenuItemRepository MenuItemRepository { get { return _menuItemRepo = new MenuItemRepository(_writeContext, _readContext); } }

        public async Task BeginAsync()
        {
            await _writeContext.BeginTransaction();
        }

        public async Task CommitAsync()
        {
            await _writeContext.SaveChangesAsync();
            await _writeContext.CommitTransaction();
        }

        public void Dispose()
        {
            _writeContext.Dispose();
        }

        public async Task RollbackAsync()
        {
            await _writeContext.RollbackTransaction();
        }
    }
}
