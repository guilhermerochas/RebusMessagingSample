using System.Reflection;
using MessagingSample.Shared.Constants;
using MessagingSample.Shared.Extensions;
using MessagingSample.Shared.Intefaces;
using Rebus.Config;
using Rebus.Serialization.Json;
using Serilog;
using ILogger = Serilog.ILogger;

var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Services.AddSingleton<ILogger>(_ => logger);

builder.Services.AddRebus(configuration =>
    {
        configuration.Transport(transport =>
        {
            transport.UseRabbitMq(builder.Configuration.GetConnectionString("RabbitMq"),
                QueueNames.ConsumerQueueName);
        });

        configuration.Logging(logging => logging.Serilog(logger));
        
        configuration.Serialization(serializer => serializer.UseSystemJson());

        configuration.Options(options =>
        {
            options.UseSimpleTopicName();
            options.UseCustomSerializer(Assembly.GetExecutingAssembly().GetTypes());
        });

        return configuration;
    },
    onCreated: async bus =>
    {
        await bus.MapCommandsAsync<ICommandHandler>(async type => await bus.Subscribe(type));
    }
);

builder.Services.AutoRegisterHandlersFromAssembly(typeof(Program).Assembly);

var app = builder.Build();

await app.RunAsync();