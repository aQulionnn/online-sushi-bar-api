using BLL.Dtos.WebhookEvent;
using DAL.Entities;

namespace BLL.Interfaces
{
    public interface IWebhookEventService
    {
        Task<WebhookEvent> CreateAsync(CreateWebhookEventDto createWebhookEventDto);
        Task<IEnumerable<WebhookEvent>> GetByEventType(string eventType);
    }
}
