using MassTransit;
using MessagingSample.ServiceBus.Handlers;
using Serilog;
using ILogger = Serilog.ILogger;

var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Services.AddSingleton<ILogger>(_ => logger);


builder.Services.AddMassTransit(config =>
{
    config.AddConsumer<HelloWorldCommandHandler>();
    
    config.UsingRabbitMq((_, context) =>
    {
        context.Host(new Uri(builder.Configuration.GetConnectionString("RabbitMq")));
    }); 
});

var app = builder.Build();

await app.RunAsync();