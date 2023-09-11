using Microsoft.AspNetCore.Mvc;
using NotifyMe.Core.Entities;
using NotifyMe.Core.Interfaces;

namespace NotifyMe.API.Controllers;

[Route("api/changes")]
[ApiController]
public class ChangesController : ControllerBase
{
    private readonly IChangeService _changeService;

    public ChangesController(IChangeService changeService)
    {
        _changeService = changeService;
    }

    [HttpGet("entity/fetch/all")]
    public IActionResult Index()
    {
        return Ok(_changeService.GetAllAsync().Result);
    }
    
    [HttpGet("entity/fetch/id/{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        try
        {
            var entity = await _changeService.GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound("The entity with the specified ID not found");
            }

            return Ok(entity);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("entity/create")]
    public async Task<IActionResult> Create(Change entity)
    {
        try
        {
            // var entityEvent = await _changeService.GetByIdAsync(entity.Id);
            // if (entityEvent != null)
            // {
            //     return BadRequest("The entity with the specified ID already exists");
            // }
            
            var maxId = _changeService.GetAllAsync().Result.Max(e => e.Id) ;
            if (maxId is null)
            {
                entity.Id = "1";
            }
            else 
            {
                var newId = Convert.ToInt32(maxId) + 1;
                entity.Id = newId.ToString(); 
            }
            
            await _changeService.CreateAsync(entity);
            return Ok(entity);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("entity/update/{entity}")]
    public async Task<IActionResult> Update(Change entity)
    {
        try
        {
            var entityEvent = await _changeService.GetByIdAsync(entity.Id);
            if (entityEvent != null)
            {
                return NotFound("The entity with the specified ID not found");
            }

            if (ModelState.IsValid)
            {
                await _changeService.UpdateAsync(entity);
            }

            return Ok(entity);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("entity/remove/{entity}")]
    public async Task<IActionResult> Delete(Change entity)
    {
        try
        {
            var entityEvent = await _changeService.GetByIdAsync(entity.Id);
            if (entityEvent != null)
            {
                return NotFound("The entity with the specified ID not found");
            }

            await _changeService.DeleteAsync(entity.Id);
            return Ok(entity);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("entity/remove/id/{id}")]
    public async Task<IActionResult> DeleteById(string id)
    {
        try
        {
            var entityEvent = await _changeService.GetByIdAsync(id);
            if (entityEvent != null)
            {
                return NotFound("The entity with the specified ID not found");
            }

            await _changeService.DeleteAsync(id);
            return Ok(entityEvent);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}