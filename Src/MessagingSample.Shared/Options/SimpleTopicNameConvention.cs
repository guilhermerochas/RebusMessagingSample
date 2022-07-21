using Rebus.Topic;

namespace MessagingSample.Shared.Options;

public class SimpleTopicNameConvention : ITopicNameConvention
{
    public string GetTopic(Type eventType) => eventType.Name;
}