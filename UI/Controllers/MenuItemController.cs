using BLL.Dtos.MenuItem;
using BLL.Interfaces;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace UI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuItemController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMenuItemService _menuItemService;

        public MenuItemController(IUnitOfWork unitOfWork, IMenuItemService menuItemService)
        {
            _unitOfWork = unitOfWork;
            _menuItemService = menuItemService;
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateAsync([FromBody] CreateMenuItemDto createMenuItemDto)
        {
            var menuItem = await _menuItemService.CreateAsync(createMenuItemDto);
            return Ok(menuItem);
        }

        [HttpGet]
        [Route("get/all")]
        public async Task<IActionResult> GetAllAsync()
        {
            var menuItems = await _menuItemService.GetAllAsync();
            return Ok(menuItems);
        }

        [HttpGet]
        [Route("get/{id:int}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] int id)
        {
            var menuItem = await _menuItemService.GetByIdAsync(id);
            if (menuItem == null)
                return NotFound($"MenuItem with id {id} not found");

            return Ok(menuItem);
        }

        [HttpPut]
        [Route("update/{id:int}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] int id, [FromBody] UpdateMenuItemDto updateMenuItemDto)
        {
            var menuItem = await _menuItemService.UpdateAsync(id, updateMenuItemDto);
            return Ok(menuItem);
        }

        [HttpDelete]
        [Route("delete/all")]
        public async Task<IActionResult> DeleteAllAsync()
        {
            var menuItems = await _menuItemService.DeleteAllAsync();
            return Ok(menuItems);
        }

        [HttpDelete]
        [Route("delete/{id:int}")]
        public async Task<IActionResult> DeleteByIdAsync([FromRoute] int id)
        {
            var menuItem = await _menuItemService.DeleteByIdAsync(id);
            if (menuItem == null) 
                return NotFound($"MenuItem with id {id} not found");

            return Ok(menuItem);
        }
    }
}
