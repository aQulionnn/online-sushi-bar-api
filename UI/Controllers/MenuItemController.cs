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
        private readonly IMenuItemRepository _menuItemRepo;

        public MenuItemController(IMenuItemRepository menuItemRepo)
        {
            _menuItemRepo = menuItemRepo;
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateAsync([FromBody] MenuItem menuItem)
        {
            var createdMenuItem = await _menuItemRepo.CreateAsync(menuItem);
            return Ok(createdMenuItem);
        }

        [HttpGet]
        [Route("get/all")]
        public async Task<IActionResult> GetAllAsync()
        {
            var menuItems = await _menuItemRepo.GetAllAsync();
            return Ok(menuItems);
        }

        [HttpGet]
        [Route("get/{id:int}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] int id)
        {
            var menuItem = await _menuItemRepo.GetByIdAsync(id);
            if (menuItem == null) 
                return NotFound($"MenuItem with id {id} not found");

            return Ok(menuItem);
        }

        [HttpPut]
        [Route("update/{id:int}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] int id, [FromBody] MenuItem menuItem)
        {
            var updatedMenuItem = await _menuItemRepo.UpdateAsync(id, menuItem);
            if (updatedMenuItem == null)
                return NotFound($"MenuItem with id {id} not found");

            return Ok(updatedMenuItem);
        }

        [HttpDelete]
        [Route("delete/all")]
        public async Task<IActionResult> DeleteAllAsync()
        {
            var deletedMenuItems = await _menuItemRepo.DeleteAllAsync();
            return Ok(deletedMenuItems);
        }

        [HttpDelete]
        [Route("delete/{id:int}")]
        public async Task<IActionResult> DeleteByIdAsync([FromRoute] int id)
        {
            var deletedMenuItem = await _menuItemRepo.DeleteByIdAsync(id);
            if (deletedMenuItem == null) 
                return NotFound($"MenuItem with id {id} not found");

            return Ok(deletedMenuItem);
        }
    }
}
