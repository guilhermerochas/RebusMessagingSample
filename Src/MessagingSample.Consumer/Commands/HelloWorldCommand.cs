using MessagingSample.Shared.Intefaces;

namespace MessagingSample.Consumer.Commands;

public class HelloWorldCommand : ICommandHandler
{
    public string? Message { get; set; } = string.Empty;
}