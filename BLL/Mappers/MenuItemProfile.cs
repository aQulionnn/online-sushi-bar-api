using AutoMapper;
using BLL.Dtos.MenuItem;
using DAL.Entities;

namespace BLL.Mapper
{
    public class MenuItemProfile : Profile
    {
        public MenuItemProfile()
        {
            CreateMap<CreateMenuItemDto, MenuItem>();
            CreateMap<UpdateMenuItemDto, MenuItem>();
            CreateMap<MenuItem, GetMenuItemDto>().ForMember(dest => dest.Category, src => src.MapFrom(x => x.Category.ToString()));
        }
    }
}
