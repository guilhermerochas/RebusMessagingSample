using System.Reflection;
using MessagingSample.Producer.Commands;
using MessagingSample.Shared.Constants;
using MessagingSample.Shared.Extensions;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using Rebus.Serialization.Json;
using Serilog;
using ILogger = Serilog.ILogger;

var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Services.AddSingleton<ILogger>(_ => logger);
builder.Logging.AddSerilog(logger);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddRebus(configuration =>
    {
        configuration.Transport(transport =>
        {
            transport.UseRabbitMq(builder.Configuration.GetConnectionString("RabbitMq"),
                QueueNames.ProducerQueueName);
        });
        
        configuration.Serialization(serializer => serializer.UseSystemJson());
        configuration.Logging(logging => logging.Serilog(logger));

        configuration.Options(options =>
        {
            options.UseSimpleTopicName();
            options.UseCustomSerializer(Assembly.GetExecutingAssembly().GetTypes());
        });

        configuration.Routing(router =>
        {
            router.TypeBased().Map<HelloWorldCommand>(QueueNames.ConsumerQueueName);
        });

        return configuration;
    }
);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();