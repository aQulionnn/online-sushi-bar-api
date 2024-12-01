using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL.Data
{
    public class AppDbContext : DbContext 
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }

        public DbSet<MenuItem> MenuItems { get; set; }

        public async Task BeginTransaction () => await Database.BeginTransactionAsync();
        public async Task CommitTransaction () => await Database.CommitTransactionAsync();
        public async Task RollbackTransaction () => await Database.RollbackTransactionAsync();
    }
}
