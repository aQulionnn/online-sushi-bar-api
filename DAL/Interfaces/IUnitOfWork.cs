using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IMenuItemRepository MenuItemRepository { get; }

        Task BeginAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}
