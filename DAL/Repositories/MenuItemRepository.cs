using DAL.Data;
using DAL.Entities;
using DAL.Interfaces;

namespace DAL.Repositories
{
    public class MenuItemRepository : BaseRepository<MenuItem>, IMenuItemRepository
    {
        private readonly AppWriteDbContext _writeContext;
        private readonly AppReadDbContext _readContext;

        public MenuItemRepository(AppWriteDbContext writeContext, AppReadDbContext readContext) : base(writeContext, readContext)
        {
            _writeContext = writeContext;
            _readContext = readContext;
        }
    }
}
