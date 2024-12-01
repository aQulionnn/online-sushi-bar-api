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

        public MenuItemController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateAsync([FromBody] MenuItem menuItem)
        {
            await _unitOfWork.BeginAsync();
            try
            {
                var createdMenuItem = await _unitOfWork.MenuItemRepository.CreateAsync(menuItem);
                await _unitOfWork.CommitAsync();
                return Ok(createdMenuItem);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("get/all")]
        public async Task<IActionResult> GetAllAsync()
        {
            var menuItems = await _unitOfWork.MenuItemRepository.GetAllAsync();
            return Ok(menuItems);
        }

        [HttpGet]
        [Route("get/{id:int}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] int id)
        {
            var menuItem = await _unitOfWork.MenuItemRepository.GetByIdAsync(id);
            if (menuItem == null)
                return NotFound($"MenuItem with id {id} not found");

            return Ok(menuItem);
        }

        [HttpPut]
        [Route("update/{id:int}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] int id, [FromBody] MenuItem menuItem)
        {
            await _unitOfWork.BeginAsync();
            try
            {
                var updatedMenuItem = _unitOfWork.MenuItemRepository.UpdateAsync(id, menuItem);
                if (updatedMenuItem == null)
                    return NotFound($"MenuItem with id {id} not found");

                await _unitOfWork.CommitAsync();
                return Ok(updatedMenuItem);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return BadRequest(ex.Message);
            }     
        }

        [HttpDelete]
        [Route("delete/all")]
        public async Task<IActionResult> DeleteAllAsync()
        {
            var menuItems = await _unitOfWork.MenuItemRepository.DeleteAllAsync();
            return Ok(menuItems);
        }

        [HttpDelete]
        [Route("delete/{id:int}")]
        public async Task<IActionResult> DeleteByIdAsync([FromRoute] int id)
        {
            var menuItem = await _unitOfWork.MenuItemRepository.DeleteByIdAsync(id);
            if (menuItem == null) 
                return NotFound($"MenuItem with id {id} not found");

            return Ok(menuItem);
        }
    }
}
