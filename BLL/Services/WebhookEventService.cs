using AutoMapper;
using BLL.Dtos.WebhookEvent;
using BLL.Interfaces;
using DAL.Entities;
using DAL.Interfaces;

namespace BLL.Services
{
    public class WebhookEventService : IWebhookEventService
    {
        private readonly IWebhookEventRepository _webhookEventRepo;
        private readonly IMapper _mapper;

        public WebhookEventService(IWebhookEventRepository webhookEventRepo, IMapper mapper)
        {
            _webhookEventRepo = webhookEventRepo;
            _mapper = mapper;
        }

        public async Task<WebhookEvent> CreateAsync(CreateWebhookEventDto createWebhookEventDto)
        {
            var webhookEvent = _mapper.Map<WebhookEvent>(createWebhookEventDto);
            var createdWebhookEvent = await _webhookEventRepo.Create(webhookEvent);
            return createdWebhookEvent;
        }

        public async Task<IEnumerable<WebhookEvent>> GetByEventType(string eventType)
        {
            return await _webhookEventRepo.GetByEventType(eventType);
        }
    }
}
