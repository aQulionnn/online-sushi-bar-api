using AutoMapper;
using BLL.Dtos.WebhookEvent;
using DAL.Entities;

namespace BLL.Mappers
{
    public class WebhookEventProfile : Profile
    {
        public WebhookEventProfile()
        {
            CreateMap<WebhookEvent, CreateWebhookEventDto>().ReverseMap();
        }
    }
}
