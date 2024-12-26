using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL.Data
{
    public class AppReadDbContext : DbContext
    {
        public AppReadDbContext(DbContextOptions<AppReadDbContext> options) : base (options)
        {
            
        }

        public DbSet<MenuItem> MenuItems { get; set; }
    }
}
