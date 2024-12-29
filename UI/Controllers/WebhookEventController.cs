using BLL.Dtos.WebhookEvent;
using BLL.Interfaces;
using DAL.Entities;
using Microsoft.AspNetCore.Mvc;
using Polly.Registry;

namespace UI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebhookEventController : ControllerBase
    {
        private readonly IWebhookEventService _webhookEventService;
        private readonly ResiliencePipelineProvider<string> _resiliencePipelineProvider;

        public WebhookEventController(IWebhookEventService webhookEventService, ResiliencePipelineProvider<string> resiliencePipelineProvider)
        {
            _webhookEventService = webhookEventService;
            _resiliencePipelineProvider = resiliencePipelineProvider;
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create([FromBody] CreateWebhookEventDto createWebhookEventDto)
        {
            var createdWebhookEvent = await _webhookEventService.CreateAsync(createWebhookEventDto);
            return Ok(createdWebhookEvent);
        }

        [HttpGet]
        [Route("{eventType}")]
        public async Task<IActionResult> GetByEventTypeAsync([FromRoute] string eventType)
        {
            var pipeline = _resiliencePipelineProvider.GetPipeline<IEnumerable<WebhookEvent>>("webhook-events-fallback");

            var webhookEvents = await pipeline.ExecuteAsync(async token => await _webhookEventService.GetByEventType(eventType));
            return Ok(webhookEvents);
        }
    }
}
