using BLL.Dtos.WebhookEvent;
using BLL.Interfaces;
using DAL.Entities;
using MediatR;

namespace Application.Features.WebhookEvents.Commands
{
    public class CreateWebhookEventCommandHandler : IRequestHandler<CreateWebhookEventCommand, WebhookEvent>
    {
        private readonly IWebhookEventService _webhookEventService;

        public CreateWebhookEventCommandHandler(IWebhookEventService webhookEventService)
        {
            _webhookEventService = webhookEventService;
        }

        public async Task<WebhookEvent> Handle(CreateWebhookEventCommand request, CancellationToken cancellationToken)
        {
            return await _webhookEventService.CreateAsync(request.CreateWebhookEventDto);
        }
    }

    public record CreateWebhookEventCommand(CreateWebhookEventDto CreateWebhookEventDto) : IRequest<WebhookEvent>;
}
