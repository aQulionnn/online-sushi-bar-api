namespace Application.Interfaces
{
    public interface IRedisService
    {
        Task<T> GetDataAsync<T>(string key);
        Task SetDataAsync<T>(string key, T data);
        Task DeleteDataAsync<T>(string key);
    }
}
