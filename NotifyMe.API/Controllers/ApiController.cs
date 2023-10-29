using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

    [HttpGet("list")]
    public async Task<IActionResult> List()
    {
        var entities = await GetListChangesAsync();
        return Ok(entities);
    }

    [HttpGet("id/{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        try
        {
            var entity = await GetChangeAsync(id);
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

    [HttpPost("{entity}")]
    public async Task<IActionResult> Create(Change entity)
    {
        try
        {
            var sequence = await GetListChangesAsync();
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

    [HttpPut("{entity}")]
    public async Task<IActionResult> Update(Change entity)
    {
        try
        {
            if (IsChangeExist(entity.Id))
                return NotFound("The entity with the specified ID not found");

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

    [HttpDelete("{entity}")]
    public async Task<IActionResult> Delete(Change entity)
    {
        try
        {
            if (IsChangeExist(entity.Id))
                return NotFound("The entity with the specified ID not found");

            await _changeService.DeleteAsync(entity.Id);
            return Ok(entity);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("id/{id}")]
    public async Task<IActionResult> DeleteById(string id)
    {
        try
        {
            if (IsChangeExist(id)) 
                return NotFound("The entity with the specified ID not found");

            await _changeService.DeleteAsync(id);
            return Ok("The entity with the specified ID was deleted");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    private Task<Change?> GetChangeAsync(string entityId)
    {
        return _changeService!
            .AsQueryable()
            .FirstOrDefaultAsync(e => e.Id == entityId);
    }

    private Task<List<Change>> GetListChangesAsync()
    {
        return _changeService!
            .AsQueryable()
            .ToListAsync();
    }

    private bool IsChangeExist(string id)
    {
        var entityEvent = GetChangeAsync(id);
        if (entityEvent is not null)
        {
            return true;
        }
        return false;
    }
}