using BLL.Interfaces;
using DAL.Entities;
using MediatR;

namespace Application.Features.WebhookEvents.Queries
{
    public class GetWebhookEventsByEventTypeQueryHandler : IRequestHandler<GetWebhookEventsByEventTypeQuery, IEnumerable<WebhookEvent>>
    {
        private readonly IWebhookEventService _webhookEventService;

        public GetWebhookEventsByEventTypeQueryHandler(IWebhookEventService webhookEventService)
        {
            _webhookEventService = webhookEventService;
        }

        public async Task<IEnumerable<WebhookEvent>> Handle(GetWebhookEventsByEventTypeQuery request, CancellationToken cancellationToken)
        {
            return await _webhookEventService.GetByEventType(request.eventType);
        }
    }

    public record GetWebhookEventsByEventTypeQuery(string eventType) : IRequest<IEnumerable<WebhookEvent>>;
}
