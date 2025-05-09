﻿using DAL.Entities;
using DAL.Parameters;
using DAL.SharedKernels;

namespace DAL.Interfaces
{
    public interface IMenuItemRepository : IBaseRepository<MenuItem>
    {
        Task<IEnumerable<MenuItem>> GetAllWithSortingAsync(SortingParameters sorting);
        Task<CursorPagedResult<MenuItem>> GetAllWithCursorPaginationAsync(CursorPaginationParameters cursorPaginationParameters);
    }
}
