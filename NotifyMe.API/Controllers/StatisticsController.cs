using Microsoft.AspNetCore.Mvc;

namespace NotifyMe.API.Controllers;

public class StatisticsController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}