using BLL.Dtos.WebhookEvent;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace UI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebhookEventController : ControllerBase
    {
        private readonly IWebhookEventService _webhookEventService;

        public WebhookEventController(IWebhookEventService webhookEventService)
        { 
            _webhookEventService = webhookEventService;
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
            var webhookEvents = await _webhookEventService.GetByEventType(eventType);
            return Ok(webhookEvents);
        }
    }
}
