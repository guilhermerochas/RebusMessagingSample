using MessagingSample.Shared.Options;
using MessagingSample.Shared.Serializers;
using MessagingSample.Shared.Utils;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Injection;
using Rebus.Serialization;
using Rebus.Topic;

namespace MessagingSample.Shared.Extensions;

public static class BusExtensions
{
    public static async Task MapCommandsAsync<TCommand>(this IBus _, Func<Type, Task> busFunction)
        where TCommand : class
    {
        var commands = ServiceUtils.GetGenericTypes<TCommand>();

        var subscribeMethod = typeof(IBus).GetMethods().FirstOrDefault(
            x => x.Name.Equals(nameof(IBus.Subscribe), StringComparison.OrdinalIgnoreCase) && x.IsGenericMethod);

        if (subscribeMethod is null)
        {
            throw new MethodAccessException($"Not able to find {nameof(IBus.Subscribe)} method");
        }

        foreach (var typeCommand in commands)
        {
            await busFunction.Invoke(typeCommand);
        }
    }

    public static void UseCustomSerializer(this OptionsConfigurer configurer, Type[] assemblyTypes)
    {
        configurer.Decorate<ISerializer>(_ => new CustomDefaultSerializer(assemblyTypes));
    }

    public static void UseSimpleTopicName(this OptionsConfigurer optionsConfigurer)
    {
        optionsConfigurer.Decorate<ITopicNameConvention>(_ => new SimpleTopicNameConvention());
    }
}