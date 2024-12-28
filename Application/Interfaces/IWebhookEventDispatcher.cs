namespace Application.Interfaces
{
    public interface IWebhookEventDispatcher
    {
        Task DispatchAsync(string eventType, object payload);
    }
}
