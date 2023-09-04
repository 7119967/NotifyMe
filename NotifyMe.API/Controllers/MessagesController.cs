using Microsoft.AspNetCore.Mvc;
using NotifyMe.Core.Interfaces.Services;

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
}