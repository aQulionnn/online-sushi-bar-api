using DAL.Data;
using DAL.Entities;
using DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class MenuItemRepository : BaseRepository<MenuItem>, IMenuItemRepository
    {
        private readonly AppDbContext _context;

        public MenuItemRepository(AppDbContext context) : base(context) 
        {
            _context = context;
        }
    }
}
