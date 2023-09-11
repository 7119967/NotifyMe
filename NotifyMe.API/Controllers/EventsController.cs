using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NotifyMe.Core.Entities;
using NotifyMe.Core.Interfaces;
using NotifyMe.Infrastructure.Context;

namespace NotifyMe.API.Controllers;

[Authorize]
public class EventsController : Controller
{
    private readonly IMapper _mapper;
    private readonly IEventService _eventService;
    private readonly UserManager<User> _userManager;
    private readonly DatabaseContext _databaseContext;
    
    public EventsController(UserManager<User> userManager, 
        IMapper mapper, 
        IEventService eventService,
        DatabaseContext databaseContext)
    {
        _userManager = userManager;
        _eventService = eventService;
        _mapper = mapper;
        _databaseContext = databaseContext;
    }
    public IActionResult Index()
    {
        return Ok(_eventService.GetAllAsync().Result);
    }
    // public async Task<IActionResult> Index()
    // {
    //     var entities = await Task.Run(() => _eventService.GetAllAsync().Result);
    //     // var model = _mapper.Map<List<GroupListViewModel>>(entities);
    //     await Task.Yield();
    //     return View(entities);
    // }
    
    // [HttpGet]
    // public async Task<IActionResult> Create()
    // {
    //     //var user = await _userManager.FindByIdAsync(_userManager.GetUserId(User));
    //     await Task.Yield();
    //     return PartialView("PartialViews/CreatePartialView", new GroupCreateViewModel());
    // }
    //
    // [HttpPost]
    // [ValidateAntiForgeryToken]
    // public async Task<IActionResult> Create(GroupCreateViewModel? model)
    // {
    //     try
    //     {
    //         if (model != null)
    //         {
    //             var maxId = _eventService.GetAllAsync().Result.Max(g => g.Id);
    //             var entity = _mapper.Map<GroupCreateViewModel, Group>(model);
    //             if (maxId is null)
    //             {
    //                 entity.Id = "1";
    //             }
    //             else 
    //             {
    //                 var newId = Convert.ToInt32(maxId) + 1;
    //                 entity.Id = newId.ToString(); 
    //             }
    //            
    //             await _eventService.CreateAsync(entity);
    //         }
    //         
    //         return RedirectToAction("Index");
    //     }
    //     catch (Exception e)
    //     {
    //         Console.WriteLine(e.Message, e);
    //         return RedirectToAction("Index");
    //     }
    // }
    //
    // [HttpGet]
    // public async Task<IActionResult> Details(string entityId)
    // {
    //     var entity = _eventService.GetAllAsync().Result.FirstOrDefault(e => e.Id == entityId);
    //     if (entity is null)
    //     {
    //         NotFound();
    //         return RedirectToAction("Index");
    //     }
    //
    //     //var model = _mapper.Map<Group, GroupDetailsViewModel>(entity);
    //     entity.Users = _databaseContext.Users.Where(e => e.GroupId == entityId).ToList();
    //
    //     await Task.Yield();
    //     return PartialView("PartialViews/DetailsPartialView", entity);
    // }
    //
    // [HttpGet]
    // public async Task<IActionResult> Edit(string entityId)
    // {
    //     var entity = _eventService.GetAllAsync().Result.FirstOrDefault(e => e.Id == entityId);
    //     if (entity == null)
    //     {
    //         return NotFound();
    //     }
    //
    //     var model = _mapper.Map<Group, GroupEditViewModel>(entity);
    //     
    //     await Task.Yield();
    //     return PartialView("PartialViews/EditPartialView", model);
    // }
    //
    // [HttpPost]
    // public async Task<IActionResult> Edit(GroupEditViewModel model)
    // {
    //     var entity = _mapper.Map<GroupEditViewModel, Group>(model);
    //     try
    //     {
    //         await _eventService.UpdateAsync(entity);
    //         return RedirectToAction("Index");
    //     }
    //     catch (Exception e)
    //     {
    //         Console.WriteLine(e.Message);
    //         return BadRequest();
    //     }
    // }
    //
    // [HttpGet]
    // public async Task<IActionResult> Delete(string entityId)
    // {
    //     var entity = _eventService.GetAllAsync().Result.FirstOrDefault(e => e.Id == entityId);
    //     if (entity == null)
    //     {
    //         return NotFound();
    //     }
    //
    //     var model = _mapper.Map<Group, GroupDeleteViewModel>(entity);
    //     
    //     await Task.Yield();
    //     return PartialView("PartialViews/DeletePartialView", model);
    // }
    //
    // [HttpPost]
    // public async Task<IActionResult> Delete(GroupDeleteViewModel model)
    // {
    //     var entity = _eventService.GetAllAsync().Result.FirstOrDefault(e => e.Id == model.Id);
    //     if (entity == null)
    //     {
    //         return NotFound();
    //     }
    //
    //     try
    //     {
    //         await _eventService.DeleteAsync(entity.Id);
    //         return RedirectToAction("Index");
    //     }
    //     catch (Exception e)
    //     {
    //         Console.WriteLine(e.Message);
    //         return BadRequest();
    //     }
    // }
}