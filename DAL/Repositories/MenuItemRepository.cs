using DAL.Data;
using DAL.Entities;
using DAL.Interfaces;
using DAL.Parameters;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IEnumerable<MenuItem>> GetAllWithSortingAsync(SortingParameters sorting)
        {
            var menuItems = _readContext.MenuItems.AsQueryable();

            if (sorting.ByName) 
                return sorting.IsAscending 
                    ? await menuItems.OrderBy(o => o.Name).ToListAsync() 
                    : await menuItems.OrderByDescending(o => o.Name).ToListAsync();
            else if (sorting.ByPrice)
                return sorting.IsAscending
                    ? await menuItems.OrderBy(o => o.Price).ToListAsync()
                    : await menuItems.OrderByDescending(o => o.Price).ToListAsync();

            return await menuItems.ToListAsync();
        }
    }
}
