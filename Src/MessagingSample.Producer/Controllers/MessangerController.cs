using MessagingSample.Producer.Commands;
using Microsoft.AspNetCore.Mvc;
using Rebus.Bus;
using ILogger = Serilog.ILogger;

namespace MessagingSample.Producer.Controllers;

[Route("[controller]/[action]")]
[ApiController]
public class MessangerController : ControllerBase
{
    private readonly ILogger _logger;
    private readonly IBus _serviceBus;
    
    public MessangerController(ILogger logger, IBus serviceBus)
    {
        _logger = logger;
        _serviceBus = serviceBus;
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage([FromQuery] string? message)
    {
        if (string.IsNullOrEmpty(message))
        {
            return BadRequest("message must not be null!");
        }

        await _serviceBus.Send(new HelloWorldCommand
        {
            Message = message
        });
        
        _logger.Information("Message sent successfully!");

        return Ok();
    }
}