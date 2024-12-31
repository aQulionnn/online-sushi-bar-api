using Application.Features.WebhookEvents.Commands;
using Application.Features.WebhookEvents.Queries;
using BLL.Dtos.WebhookEvent;
using DAL.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Polly.Registry;

namespace UI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebhookEventController : ControllerBase
    {
        private readonly ISender _sender;
        private readonly ResiliencePipelineProvider<string> _resiliencePipelineProvider;

        public WebhookEventController(ResiliencePipelineProvider<string> resiliencePipelineProvider, ISender sender)
        { 
            _resiliencePipelineProvider = resiliencePipelineProvider;
            _sender = sender;
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create([FromBody] CreateWebhookEventDto createWebhookEventDto)
        {
            var command = new CreateWebhookEventCommand(createWebhookEventDto);
            var createdWebhookEvent = await _sender.Send(command);

            return Ok(createdWebhookEvent);
        }

        [HttpGet]
        [Route("{eventType}")]
        public async Task<IActionResult> GetByEventTypeAsync([FromRoute] string eventType)
        {
            var query = new GetWebhookEventsByEventTypeQuery(eventType);

            var pipeline = _resiliencePipelineProvider.GetPipeline<IEnumerable<WebhookEvent>>("webhook-events-fallback");
            var webhookEvents = await pipeline.ExecuteAsync(async token => await _sender.Send(query));
            
            return Ok(webhookEvents);
        }
    }
}
