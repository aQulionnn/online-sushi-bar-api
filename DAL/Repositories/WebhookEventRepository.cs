using DAL.Data;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class WebhookEventRepository : IWebhookEventRepository
    {
        private readonly AppWriteDbContext _writeContext;
        private readonly AppReadDbContext _readContext;

        public WebhookEventRepository(AppWriteDbContext writeContext, AppReadDbContext readContext)
        {
            _writeContext = writeContext;
            _readContext = readContext;
        }

        public async Task<WebhookEvent> Create(WebhookEvent createdWebhookEvent)
        {
            await _writeContext.WebhookEvents.AddAsync(createdWebhookEvent);
            await _writeContext.SaveChangesAsync();
            return createdWebhookEvent;
        }

        public async Task<IEnumerable<WebhookEvent>> GetByEventType(string eventType)
        {
            return await _readContext.WebhookEvents.Where(x => x.EventType == eventType).ToListAsync();
        }
    }
}
