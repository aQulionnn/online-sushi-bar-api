using DAL.Data;
using DAL.Entities;
using DAL.Interfaces;
using DAL.Parameters;
using DAL.SharedKernels;
using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;

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

        public async Task<IEnumerable<MenuItem>> GetBySearchTerm(string searchTerm)
        {
            var menuItems = await _readContext.MenuItems
                .Where(m => 
                    EF.Functions
                        .ToTsVector("english", m.Name + " " + m.Description)
                        .Matches(EF.Functions.PhraseToTsQuery("english", searchTerm)))
                .ToListAsync();
            
            return menuItems;
        }

        public async Task<IEnumerable<MenuItem>> GetBySearchTermWithRank(string searchTerm)
        {
            var menuItems = await _readContext.MenuItems
                .Where(m => 
                    EF.Functions
                        .ToTsVector("english", m.Name + " " + m.Description)
                        .Matches(EF.Functions.PhraseToTsQuery("english", searchTerm)))
                .OrderByDescending(m => 
                    EF.Functions.ToTsVector("english", m.Name + " " + m.Description)
                        .Rank(EF.Functions.PhraseToTsQuery("english", searchTerm)))
                .ToListAsync();
            
            return menuItems;
        }

        public async Task<IEnumerable<MenuItem>> GetByWeightedSearchTermWithRank(string searchTerm)
        {
            var menuItems = await _readContext.MenuItems
                .Where(m => 
                    EF.Functions
                        .ToTsVector("english", m.Name)
                            .SetWeight(NpgsqlTsVector.Lexeme.Weight.A)
                        .Concat(EF.Functions.ToTsVector("english", m.Description)
                            .SetWeight(NpgsqlTsVector.Lexeme.Weight.B))
                        .Matches(EF.Functions.PhraseToTsQuery("english", searchTerm)))
                .OrderByDescending(m =>
                    EF.Functions
                        .ToTsVector("english", m.Name)
                            .SetWeight(NpgsqlTsVector.Lexeme.Weight.A)
                        .Concat(EF.Functions.ToTsVector("english", m.Description)
                            .SetWeight(NpgsqlTsVector.Lexeme.Weight.B))
                        .Rank(EF.Functions.PhraseToTsQuery("english", searchTerm)))
                .ToListAsync();
            
            return menuItems;
        }
    }
}
