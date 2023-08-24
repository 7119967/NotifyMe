using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NotifyMe.Core.Entities;
using NotifyMe.Core.Interfaces.Services;

namespace NotifyMe.API.Controllers;

public class GroupsController : Controller
{
    private readonly IMapper _mapper;
    private readonly IGroupService _groupService;

    public GroupsController(IMapper mapper,
        IGroupService groupService)
    {
        _groupService = groupService;
        _mapper = mapper;
    }
    public IActionResult Index()
    {
        var groups = _mapper.Map<List<Group>>(_groupService.GetAllAsync().Result);

        return View(groups);
    }
}