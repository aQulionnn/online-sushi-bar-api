using Application.Features.MenuItem.Commands;
using Application.Features.MenuItem.Queries;
using Application.Interfaces;
using BLL.Dtos.MenuItem;
using DAL.Parameters;
using DAL.SharedKernels;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Polly.Registry;
using System;
using System.Threading.Tasks;

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
            var result = await _sender.Send(command);

            await _webhookEventDispatcher.DispatchAsync("MenuItem.created", result);

            return StatusCode((int)result.Error.StatusCode, result);
        }

        [HttpGet]
        [Route("get/all")]
        [EnableRateLimiting("GetRequestLimiter")]
        public async Task<IActionResult> GetAllAsync([FromQuery] PaginationParameters pagination)
        {
            var query = new GetAllMenuItemQuery(pagination);
            var result = await _sender.Send(query);

            return StatusCode((int)result.Error.StatusCode, result);
        }

        [HttpGet]
        [Route("get/all-with-sorting")]
        [EnableRateLimiting("GetRequestLimiter")]
        public async Task<IActionResult> GetAllWithSortingAsync([FromQuery] SortingParameters sorting)
        {
            var query = new GetSortedMenuItemsQuery(sorting);
            var result = await _sender.Send(query);

            return StatusCode((int)result.Error.StatusCode, result);
        }

        [HttpGet]
        [Route("get/all-with-cursor")]
        [EnableRateLimiting("GetRequestLimiter")]
        public async Task<IActionResult> GetAllWithCursorPaginationAsync([FromQuery] CursorPaginationParameters pagination)
        {
            var query = new GetAllWithCursorPaginationQuery(pagination);
            var result = await _sender.Send(query);
            
            return StatusCode((int)result.Error.StatusCode, result);
        }

        [HttpGet]
        [Route("get/{id:int}")]
        [EnableRateLimiting("GetRequestLimiter")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] int id)
        {
            var pipeline = _resiliencePipelineProvider.GetPipeline<Result<GetMenuItemDto>?>("menu-items-fallback");

            var query = new GetMenuItemByIdQuery(id);
            var result = await pipeline.ExecuteAsync(async token => await _sender.Send(query));

            return StatusCode((int)result.Error.StatusCode, result);
        }
        
        [HttpGet]
        [Route("get/search")]
        public async Task<IActionResult> GetBySearchTermAsync([FromQuery] string searchTerm)
        {
            var query = new GetBySearchTermQuery(searchTerm);
            var result = await _sender.Send(query);
            
            return StatusCode((int)result.Error!.StatusCode, result);
        }

        [HttpPut]
        [Route("update/{id:int}")]
        [EnableRateLimiting("PutRequestLimiter")]
        public async Task<IActionResult> UpdateAsync([FromRoute] int id, [FromBody] UpdateMenuItemDto updateMenuItemDto)
        {
            var command = new UpdateMenuItemCommand(id, updateMenuItemDto);
            var result = await _sender.Send(command);

            await _webhookEventDispatcher.DispatchAsync("MenuItem.updated", result);

            return StatusCode((int)result.Error.StatusCode, result);
        }

        [HttpDelete]
        [Route("delete/all")]
        [EnableRateLimiting("DeleteRequestLimiter")]
        public async Task<IActionResult> DeleteAllAsync()
        {
            var command = new DeleteAllMenuItemCommand();
            var result = await _sender.Send(command);

            await _webhookEventDispatcher.DispatchAsync("MenuItems.deleted", result);

            return StatusCode((int)result.Error.StatusCode, result);
        }

        [HttpDelete]
        [Route("delete/{id:int}")]
        [EnableRateLimiting("DeleteRequestLimiter")]
        public async Task<IActionResult> DeleteByIdAsync([FromRoute] int id)
        {
            var command = new DeleteMenuItemByIdCommand(id);
            var result = await _sender.Send(command);

            await _webhookEventDispatcher.DispatchAsync("MenuItem.deleted", result);

            return StatusCode((int)result.Error.StatusCode, result);
        }
    }
}
