using Microsoft.AspNetCore.Mvc;
using NotifyMe.Core.Entities;
using NotifyMe.Core.Interfaces.Services;
using NotifyMe.Infrastructure.Services;

namespace NotifyMe.API.Controllers;

[Route("api/changes")]
[ApiController]
public class ApiController : ControllerBase
{
    private readonly IChangeService _changeService;

    public ApiController(IChangeService changeService)
    {
        _changeService = changeService;
    }

    [HttpGet("entity/fetch/list")]
    public IActionResult List()
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
            var sequence = _changeService.GetAllAsync().Result.ToList();
            // var newId = (sequence?.Any() == true) ? (sequence.Max(e => Convert.ToInt32(e.Id)) + 1) : 1;
            var newId = Helpers.GetNewIdEntity(sequence);
            entity.Id = newId.ToString();
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