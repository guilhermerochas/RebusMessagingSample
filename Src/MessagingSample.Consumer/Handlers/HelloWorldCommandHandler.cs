using MessagingSample.Consumer.Commands;
using Rebus.Handlers;
using ILogger = Serilog.ILogger;

namespace MessagingSample.Consumer.Handlers;

public class HelloWorldCommandHandler : IHandleMessages<HelloWorldCommand>
{
    private readonly ILogger _logger;

    public HelloWorldCommandHandler(ILogger logger)
    {
        _logger = logger;
    }

    public Task Handle(HelloWorldCommand command)
    {
        _logger.Information("Mensagem enviada foi: {Message}", command.Message);
        return Task.CompletedTask;
    }
}