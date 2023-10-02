using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotifyMe.Core.Interfaces.Services;

namespace NotifyMe.API.Controllers;

[Authorize(Roles = "admin, user")]
[Route("api/[controller]")]
[ApiController]
public class RabbitMqController : ControllerBase
{
    private readonly IRabbitMqPublisher _rabbitMqPublisher;

    public RabbitMqController(IRabbitMqPublisher rabbitMqPublisher)
    {
        _rabbitMqPublisher = rabbitMqPublisher;
    }

    [Route("[action]/{message}")]
    [HttpGet]
    public IActionResult SendMessage(string message)
    {
        _rabbitMqPublisher.PublishMessage(message);

        return Ok("The message was sent");
    }
}