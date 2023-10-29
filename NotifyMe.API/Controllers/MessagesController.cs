using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using NotifyMe.Core.Interfaces.Services;

using Message = NotifyMe.Core.Entities.Message;

namespace NotifyMe.API.Controllers;

[Authorize(Roles = "admin, user")]
public class MessagesController : Controller
{
    private readonly IMessageService _messageService;

    public MessagesController(IMessageService messageService)
    {
        _messageService = messageService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var entities = await GetListMessagesAsync();
        return View(entities);
    }

    [HttpGet]
    public async Task<IActionResult> Details(string entityId)
    {
        var entity = await GetMessageAsync(entityId);
        if (entity is null)
        {
            NotFound();
            return RedirectToAction("Index");
        }

        return PartialView("PartialViews/DetailsPartialView", entity);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(string entityId)
    {
        var entity = await GetMessageAsync( entityId);
        if (entity == null)
        {
            return NotFound();
        }

        return PartialView("PartialViews/DeletePartialView", entity);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(Message model)
    {
        var entity = await GetMessageAsync(model.Id);
        if (entity == null)
        {
            return NotFound();
        }

        try
        {
            await _messageService.DeleteAsync(entity.Id);
            return RedirectToAction("Index");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return BadRequest();
        }
    }

    private Task<Message?> GetMessageAsync(string entityId)
    {
        return _messageService!
            .AsQueryable()
            .FirstOrDefaultAsync(e => e.Id == entityId);
    }

    private Task<List<Message>> GetListMessagesAsync()
    {
        return _messageService!
            .AsQueryable()
            .ToListAsync();
    }
}