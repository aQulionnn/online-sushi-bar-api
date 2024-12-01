using DAL.Enums;
using System.ComponentModel.DataAnnotations;

namespace DAL.Entities
{
    public class MenuItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty; 
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        [Range(0, 1, ErrorMessage = "Invalid category")]
        public MenuItemCategory Category { get; set; }
    }
}
