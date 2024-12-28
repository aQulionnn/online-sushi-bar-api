using Application.Interfaces;
using BLL.Interfaces;
using System.Net.Http.Json;

namespace Application.Services
{
    public class WebhookEventDispatcher : IWebhookEventDispatcher
    {
        private readonly HttpClient _httpClient;
        private readonly IWebhookEventService _webhookEventService;

        public WebhookEventDispatcher(HttpClient httpClient, IWebhookEventService webhookEventService)
        {
            _httpClient = httpClient;
            _webhookEventService = webhookEventService;
        }

        public async Task DispatchAsync(string eventType, object payload)
        {
            var webhookEvents = await _webhookEventService.GetByEventType(eventType);

            foreach (var webhookEvent in webhookEvents)
            {
                var request = new
                {
                    Id = Guid.NewGuid(),
                    eventType = webhookEvent.EventType,
                    WebhookEventId = webhookEvent.Id,
                    TimeStamp = webhookEvent.CreatedOn,
                    Data = payload
                };

                await _httpClient.PostAsJsonAsync(webhookEvent.WebhookUrl, request);
            }
        }
    }
}
