using Application.Features.MenuItem.Commands;
using Application.Features.MenuItem.Queries;
using BLL.Dtos.MenuItem;
using DAL.Parameters;
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
        private readonly IValidator<CreateMenuItemDto> _createValidator;
        private readonly IValidator<UpdateMenuItemDto> _updateValidator;

        public MenuItemController(ISender sender, ResiliencePipelineProvider<string> resiliencePipelineProvider, IValidator<CreateMenuItemDto> createValidator, IValidator<UpdateMenuItemDto> updateValidator)
        {
            _sender = sender;
            _resiliencePipelineProvider = resiliencePipelineProvider;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        [HttpPost]
        [Route("create")]
        [EnableRateLimiting("PostRequestLimiter")]
        public async Task<IActionResult> CreateAsync([FromBody] CreateMenuItemDto createMenuItemDto)
        {
            var validation = await _createValidator.ValidateAsync(createMenuItemDto);    
            if (!validation.IsValid)
            {
                var problemDetaisl = new HttpValidationProblemDetails(validation.ToDictionary());
                return BadRequest(problemDetaisl);
            }

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
            var validation = await _updateValidator.ValidateAsync(updateMenuItemDto);
            if (!validation.IsValid)
            {
                var problemDetails = new HttpValidationProblemDetails(validation.ToDictionary());
                return BadRequest(problemDetails);
            }

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
