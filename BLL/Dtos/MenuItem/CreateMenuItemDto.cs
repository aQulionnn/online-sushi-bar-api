using DAL.Enums;

namespace BLL.Dtos.MenuItem
{
    public class CreateMenuItemDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public MenuItemCategory Category { get; set; }
    }
}
