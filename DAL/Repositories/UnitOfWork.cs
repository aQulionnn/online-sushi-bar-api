using DAL.Data;
using DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IMenuItemRepository _menuItemRepo;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IMenuItemRepository MenuItemRepository { get { return _menuItemRepo = new MenuItemRepository(_context); } }

        public async Task BeginAsync()
        {
            await _context.BeginTransaction();
        }

        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
            await _context.CommitTransaction();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task RollbackAsync()
        {
            await _context.RollbackTransaction();
        }
    }
}
