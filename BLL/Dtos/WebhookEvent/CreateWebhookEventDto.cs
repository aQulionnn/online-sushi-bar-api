namespace BLL.Dtos.WebhookEvent
{
    public class CreateWebhookEventDto
    {
        public string EventType { get; set; } = string.Empty;
        public string WebhookUrl { get; set; } = string.Empty;
    }
}
