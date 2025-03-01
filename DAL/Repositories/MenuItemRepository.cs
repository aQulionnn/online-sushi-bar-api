using DAL.Data;
using DAL.Entities;
using DAL.Interfaces;
using DAL.Parameters;
using DAL.SharedKernels;
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

        public async Task<CursorPagedResult<MenuItem>> GetAllWithCursorPaginationAsync(CursorPaginationParameters cursorPaginationParameters)
        {
            var query = _readContext.MenuItems.AsQueryable();
            
            if (cursorPaginationParameters.LastId != null)
                query = query.Where(x => x.Id < cursorPaginationParameters.LastId);

            var menuItems = await query
                .OrderByDescending(x => x.Id)
                .Take(cursorPaginationParameters.Limit)
                .ToListAsync();
            
            bool hasMore = menuItems.Count > cursorPaginationParameters.Limit;
            int? nextLastId = hasMore ? menuItems[^1].Id : null;
            
            if (hasMore)
                menuItems.RemoveAt(menuItems.Count - 1);
            
            return new CursorPagedResult<MenuItem>(menuItems, nextLastId, hasMore);
        }
    }
}
