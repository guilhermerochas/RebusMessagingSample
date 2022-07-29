using MassTransit;
using MessagingSample.ServiceBus.Commands;
using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;

namespace MessagingSample.Producer.Controllers;

[Route("[controller]/[action]")]
[ApiController]
public class MessangerController : ControllerBase
{
    private readonly IBus _bus;
    private readonly ILogger _logger;
    
    public MessangerController(ILogger logger, IBus bus)
    {
        _logger = logger;
        _bus = bus;
    }

    [HttpPost]
    public Task<IActionResult> SendMessage([FromQuery] string? message)
    {
        if (string.IsNullOrEmpty(message))
        {
            return Task.FromResult<IActionResult>(BadRequest("message must not be null!"));
        }

        _bus.Publish(new HelloWorldCommand
        {
            Message = message
        });
        
        _logger.Information("Message sent successfully!");

        return Task.FromResult<IActionResult>(Ok());
    }
}