using Application.Features.MenuItem.Commands;
using Application.Features.MenuItem.Queries;
using BLL.Dtos.MenuItem;
using DAL.Parameters;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace UI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuItemController : ControllerBase
    {
        private readonly ISender _sender;

        public MenuItemController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateAsync([FromBody] CreateMenuItemDto createMenuItemDto)
        {
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);

            var command = new CreateMenuItemCommand(createMenuItemDto);
            var menuItem = await _sender.Send(command);
            return Ok(menuItem);
        }

        [HttpGet]
        [Route("get/all")]
        public async Task<IActionResult> GetAllAsync([FromQuery] PaginationParameters pagination)
        {
            var query = new GetAllMenuItemQuery(pagination);
            var menuItems = await _sender.Send(query);
            return Ok(menuItems);
        }

        [HttpGet]
        [Route("get/{id:int}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] int id)
        {
            var query = new GetMenuItemByIdQuery(id);
            var menuItem = await _sender.Send(query);
            if (menuItem == null)
                return NotFound($"MenuItem with id {id} not found");

            return Ok(menuItem);
        }

        [HttpPut]
        [Route("update/{id:int}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] int id, [FromBody] UpdateMenuItemDto updateMenuItemDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var command = new UpdateMenuItemCommand(id, updateMenuItemDto);
            var menuItem = await _sender.Send(command);
            return Ok(menuItem);
        }

        [HttpDelete]
        [Route("delete/all")]
        public async Task<IActionResult> DeleteAllAsync()
        {
            var command = new DeleteAllMenuItemCommand();
            var menuItems = await _sender.Send(command);
            return Ok(menuItems);
        }

        [HttpDelete]
        [Route("delete/{id:int}")]
        public async Task<IActionResult> DeleteByIdAsync([FromRoute] int id)
        {
            var command = new DeleteMenuItemByIdCommand(id);
            var menuItem = await _sender.Send(command);
            if (menuItem == null) 
                return NotFound($"MenuItem with id {id} not found");

            return Ok(menuItem);
        }
    }
}
