using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NotifyMe.Core.Entities;
using NotifyMe.Core.Enums;
using NotifyMe.Core.Interfaces;
using NotifyMe.Core.Interfaces.Services;
using NotifyMe.Infrastructure.Context;

namespace NotifyMe.API.Controllers;

[Authorize(Roles = "admin, user")]
public class ChangesController  : Controller
{
    private readonly IChangeService _changeService;
    private readonly DatabaseContext _databaseContext;
    
    public ChangesController(IChangeService changeService,
        DatabaseContext databaseContext)
    {
        _changeService = changeService;
        _databaseContext = databaseContext;
    }

    public async Task<IActionResult> Index()
    {
        var entities = await Task.Run(() => _changeService.GetAllAsync().Result);
        // var model = _mapper.Map<List<GroupListViewModel>>(entities);
        await Task.Yield();
        return View(entities);
    }
    
    [HttpGet]
    public async Task<IActionResult> Details(string entityId)
    {
        var entity = _changeService.GetAllAsync().Result.FirstOrDefault(e => e.Id == entityId);
        if (entity is null)
        {
            NotFound();
            return RedirectToAction("Index");
        }

        //var model = _mapper.Map<Group, GroupDetailsViewModel>(entity);
        // entity.Event = _databaseContext.Events.FirstOrDefault(e => e.Id == entityId);

        await Task.Yield();
        return PartialView("PartialViews/DetailsPartialView", entity);
    }
    
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var  changeTypeValues = Enum.GetValues(typeof(ChangeType)).Cast<ChangeType>();
        var changeTypes = new SelectList(changeTypeValues);
        ViewBag.ChangeTypes = changeTypes;
        
        //var user = await _userManager.FindByIdAsync(_userManager.GetUserId(User));
        await Task.Yield();
        return PartialView("PartialViews/CreatePartialView", new Change());
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Change? model)
    {
        try
        {
            if (model != null)
            {
                var seequence = _changeService.GetAllAsync().Result;
                var size = seequence.Count();
                int[] anArray = new int[size];
                if (size == 0)
                {
                    model.Id = "1";
                }
                else
                {
                    var index = 0;
                    foreach (var change in seequence)
                    {
                        anArray[index] = Convert.ToInt32(change.Id);
                        index++;
                    }

                    int maxValue = anArray.Max();
                    var newId = Convert.ToInt32(maxValue) + 1;
                    model.Id = newId.ToString();
                }

                await _changeService.CreateAsync(model);
            }
            
            return RedirectToAction("Index");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message, e);
            return RedirectToAction("Index");
        }
    }
    //
    // [HttpGet("entity/fetch/list")]
    // public IActionResult List()
    // {
    //     return Ok(_changeService.GetAllAsync().Result);
    // }
    //
    // [Authorize(Roles = "admin, user")]
    // [HttpGet]
    // public IActionResult Index()
    // { 
    //     var content = _changeService.GetAllAsync().Result;
    //     var json = JsonConvert.SerializeObject(content);
    //     return Content(json, "text/html");
    // }
    //
    // [HttpGet("entity/fetch/id/{id}")]
    // public async Task<IActionResult> GetById(string id)
    // {
    //     try
    //     {
    //         var entity = await _changeService.GetByIdAsync(id);
    //         if (entity == null)
    //         {
    //             return NotFound("The entity with the specified ID not found");
    //         }
    //
    //         return Ok(entity);
    //     }
    //     catch (Exception e)
    //     {
    //         return BadRequest(e.Message);
    //     }
    // }
    //
    // [HttpPost("entity/create")]
    // public async Task<IActionResult> Create(Change entity)
    // {
    //     try
    //     {
    //         // var entityEvent = await _changeService.GetByIdAsync(entity.Id);
    //         // if (entityEvent != null)
    //         // {
    //         //     return BadRequest("The entity with the specified ID already exists");
    //         // }
    //        
    //         var seequence = _changeService.GetAllAsync().Result;
    //         var size = seequence.Count();
    //         int[] anArray = new int[size];
    //         if (size == 0)
    //         {
    //             entity.Id = "1";
    //         }
    //         else
    //         {
    //             var index = 0;
    //             foreach (var change in seequence)
    //             {
    //                 anArray[index] = Convert.ToInt32(change.Id);
    //                 index++;
    //             }
    //
    //             int maxValue = anArray.Max();
    //             var newId = Convert.ToInt32(maxValue) + 1;
    //             entity.Id = newId.ToString();
    //         }
    //
    //
    //         //var maxId = _changeService.GetAllAsync().Result.Max(e => e.Id);
    //         //if (maxId is null)
    //         //{
    //         //    entity.Id = "1";
    //         //}
    //         //else 
    //         //{
    //         //    var newId = Convert.ToInt32(maxId) + 1;
    //         //    entity.Id = newId.ToString(); 
    //         //}
    //         
    //         await _changeService.CreateAsync(entity);
    //         return Ok(entity);
    //     }
    //     catch (Exception e)
    //     {
    //         return BadRequest(e.Message);
    //     }
    // }
    //
    // [HttpPut("entity/update/{entity}")]
    // public async Task<IActionResult> Update(Change entity)
    // {
    //     try
    //     {
    //         var entityEvent = await _changeService.GetByIdAsync(entity.Id);
    //         if (entityEvent != null)
    //         {
    //             return NotFound("The entity with the specified ID not found");
    //         }
    //
    //         if (ModelState.IsValid)
    //         {
    //             await _changeService.UpdateAsync(entity);
    //         }
    //
    //         return Ok(entity);
    //     }
    //     catch (Exception e)
    //     {
    //         return BadRequest(e.Message);
    //     }
    // }
    //
    // [HttpDelete("entity/remove/{entity}")]
    // public async Task<IActionResult> Delete(Change entity)
    // {
    //     try
    //     {
    //         var entityEvent = await _changeService.GetByIdAsync(entity.Id);
    //         if (entityEvent != null)
    //         {
    //             return NotFound("The entity with the specified ID not found");
    //         }
    //
    //         await _changeService.DeleteAsync(entity.Id);
    //         return Ok(entity);
    //     }
    //     catch (Exception e)
    //     {
    //         return BadRequest(e.Message);
    //     }
    // }
    //
    // [HttpDelete("entity/remove/id/{id}")]
    // public async Task<IActionResult> DeleteById(string id)
    // {
    //     try
    //     {
    //         var entityEvent = await _changeService.GetByIdAsync(id);
    //         if (entityEvent != null)
    //         {
    //             return NotFound("The entity with the specified ID not found");
    //         }
    //
    //         await _changeService.DeleteAsync(id);
    //         return Ok(entityEvent);
    //     }
    //     catch (Exception e)
    //     {
    //         return BadRequest(e.Message);
    //     }
    // }
}