using Application.Features.MenuItem.Commands;
using Application.Features.MenuItem.Queries;
using Application.Interfaces;
using BLL.Dtos.MenuItem;
using DAL.Parameters;
using DAL.SharedKernels;
using FluentValidation;
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
        private readonly IWebhookEventDispatcher _webhookEventDispatcher;

        public MenuItemController(ISender sender, ResiliencePipelineProvider<string> resiliencePipelineProvider, IWebhookEventDispatcher webhookEventDispatcher)
        {
            _sender = sender;
            _resiliencePipelineProvider = resiliencePipelineProvider;
            _webhookEventDispatcher = webhookEventDispatcher;
        }

        [HttpPost]
        [Route("create")]
        [EnableRateLimiting("PostRequestLimiter")]
        public async Task<IActionResult> CreateAsync([FromBody] CreateMenuItemDto createMenuItemDto)
        {
            var command = new CreateMenuItemCommand(createMenuItemDto);
            var menuItem = await _sender.Send(command);

            await _webhookEventDispatcher.DispatchAsync("MenuItem.created", menuItem);

            return menuItem.IsSuccess ? Ok(menuItem) : StatusCode((int)menuItem.Error.StatusCode, menuItem);
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
            var pipeline = _resiliencePipelineProvider.GetPipeline<Result<GetMenuItemDto>?>("menu-items-fallback");

            var query = new GetMenuItemByIdQuery(id);
            var result = await pipeline.ExecuteAsync(async token => await _sender.Send(query));

            return result.IsSuccess ? Ok(result) : StatusCode((int)result.Error.StatusCode, result);
        }

        [HttpPut]
        [Route("update/{id:int}")]
        [EnableRateLimiting("PutRequestLimiter")]
        public async Task<IActionResult> UpdateAsync([FromRoute] int id, [FromBody] UpdateMenuItemDto updateMenuItemDto)
        {
            var command = new UpdateMenuItemCommand(id, updateMenuItemDto);
            var result = await _sender.Send(command);

            return result.IsSuccess ? Ok(result) : StatusCode((int)result.Error.StatusCode, result);
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
