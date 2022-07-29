using MassTransit;
using MessagingSample.ServiceBus.Commands;
using Serilog;

namespace MessagingSample.ServiceBus.Handlers;

public class HelloWorldCommandHandler : IConsumer<HelloWorldCommand>
{
    private readonly ILogger _logger;

    public HelloWorldCommandHandler(ILogger logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<HelloWorldCommand> context)
    {
        _logger.Information("{Message}", context.Message.Message);
        return Task.CompletedTask;
    }
}