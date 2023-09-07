using Microsoft.AspNetCore.Mvc;

using NotifyMe.Core.Interfaces.Services;
using NotifyMe.Core.Models.Group;

namespace NotifyMe.API.Controllers;

public class MessagesController : Controller
{
    private readonly IMessageService _messageService;

    public MessagesController(IMessageService messageService)
    {
        _messageService = messageService;
    }
    public async Task<IActionResult> Index()
    {
        var entities = await Task.Run(() => _messageService.GetAllAsync().Result);
        return View(entities);
    }

    [HttpGet]
    public async Task<IActionResult> Details(string entityId)
    {
        var entity = _messageService.GetAllAsync().Result.FirstOrDefault(e => e.Id == entityId);
        if (entity is null)
        {
            NotFound();
            return RedirectToAction("Index");
        }

        //var model = _mapper.Map<Group, GroupDetailsViewModel>(entity);

        await Task.Yield();
        return PartialView("PartialViews/DetailsPartialView", entity);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(string entityId)
    {
        var entity = _messageService.GetAllAsync().Result.FirstOrDefault(e => e.Id == entityId);
        if (entity == null)
        {
            return NotFound();
        }

        //var model = _mapper.Map<Group, GroupDeleteViewModel>(entity);

        await Task.Yield();
        return PartialView("PartialViews/DeletePartialView", entity);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(GroupDeleteViewModel model)
    {
        var entity = _messageService.GetAllAsync().Result.FirstOrDefault(e => e.Id == model.Id);
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
}