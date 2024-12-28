namespace DAL.Entities
{
    public class WebhookEvent
    {
        public int Id { get; set; }
        public string EventType { get; set; } = string.Empty;
        public string WebhookUrl { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}
