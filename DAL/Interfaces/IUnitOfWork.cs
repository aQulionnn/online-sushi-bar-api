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
