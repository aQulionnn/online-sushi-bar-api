using Application.Features.MenuItem.Commands;
using Application.Features.MenuItem.Queries;
using BLL.Dtos.MenuItem;
using DAL.Parameters;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Polly.Registry;

namespace UI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuItemController : ControllerBase
    {
        private readonly ISender _sender;
        private readonly ResiliencePipelineProvider<string> _resiliencePipelineProvider;

        public MenuItemController(ISender sender, ResiliencePipelineProvider<string> resiliencePipelineProvider)
        {
            _sender = sender;
            _resiliencePipelineProvider = resiliencePipelineProvider;
        }

        [HttpPost]
        [Route("create")]
        [EnableRateLimiting("PostRequestLimiter")]
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
        [EnableRateLimiting("GetRequestLimiter")]
        public async Task<IActionResult> GetAllAsync([FromQuery] PaginationParameters pagination)
        {
            var query = new GetAllMenuItemQuery(pagination);
            var menuItems = await _sender.Send(query);
            return Ok(menuItems);
        }

        [HttpGet]
        [Route("get/{id:int}")]
        [EnableRateLimiting("GetRequestLimiter")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] int id)
        {
            var pipeline = _resiliencePipelineProvider.GetPipeline<GetMenuItemDto?>("menu-items-fallback");

            var query = new GetMenuItemByIdQuery(id);
            var menuItem = await pipeline.ExecuteAsync(async token => await _sender.Send(query));
            if (menuItem == null)
                return NotFound($"MenuItem with id {id} not found");

            return Ok(menuItem);
        }

        [HttpPut]
        [Route("update/{id:int}")]
        [EnableRateLimiting("PutRequestLimiter")]
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
        [EnableRateLimiting("DeleteRequestLimiter")]
        public async Task<IActionResult> DeleteAllAsync()
        {
            var command = new DeleteAllMenuItemCommand();
            var menuItems = await _sender.Send(command);
            return Ok(menuItems);
        }

        [HttpDelete]
        [Route("delete/{id:int}")]
        [EnableRateLimiting("DeleteRequestLimiter")]
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
