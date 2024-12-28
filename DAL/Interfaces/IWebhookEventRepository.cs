using DAL.Entities;

namespace DAL.Interfaces
{
    public interface IWebhookEventRepository
    {
        Task<WebhookEvent> Create(WebhookEvent createdWebhookEvent);
        Task<IEnumerable<WebhookEvent>> GetByEventType(string eventType);
    }
}
