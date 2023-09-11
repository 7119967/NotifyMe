using Microsoft.AspNetCore.Mvc;

namespace NotifyMe.API.Controllers;

public class DashboardController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}