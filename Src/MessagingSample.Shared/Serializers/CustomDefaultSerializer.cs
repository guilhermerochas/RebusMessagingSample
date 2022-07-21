using System.Reflection;
using System.Text;
using System.Text.Json;
using Rebus.Extensions;
using Rebus.Messages;
using Rebus.Serialization;

namespace MessagingSample.Shared.Serializers;

public class CustomDefaultSerializer : ISerializer
{
    private readonly string _encodingHeaderValue;
    private const string JsonContentType = "application/json";
    private readonly Encoding _defaultEncoder = Encoding.UTF8;
    private readonly Type[]? _assemblyTypes;

    public CustomDefaultSerializer(Type[]? assemblyTypes)
    {
        _assemblyTypes = assemblyTypes;
        _encodingHeaderValue = $"{JsonContentType};charset={_defaultEncoder.HeaderName}";
    }

    public Task<TransportMessage> Serialize(Message message)
    {
        var bytes = JsonSerializer.SerializeToUtf8Bytes(message.Body, message.Body.GetType());
        var headers = message.Headers.Clone();

        headers[Headers.ContentType] = _encodingHeaderValue;
        headers[Headers.Type] = message.Body.GetType().Name;

        return Task.FromResult(new TransportMessage(headers, bytes));
    }

    public Task<Message> Deserialize(TransportMessage transportMessage)
    {
        var headers = transportMessage.Headers.Clone();
        var json = _defaultEncoder.GetString(transportMessage.Body);
        var typeName = headers.GetValue(Headers.Type);

        Type? foundAssemblyType = null;

        if (_assemblyTypes is not null)
        {
            foreach (var type in _assemblyTypes)
            {
                if (!typeName.Equals(type.Name))
                {
                    continue;
                }

                foundAssemblyType = type;
                break;
            }
        }

        if (foundAssemblyType is null)
        {
            throw new NullReferenceException($"Not able to find Type: {typeName}");
        }

        return Task.FromResult(new Message(headers, JsonSerializer.Deserialize(json, foundAssemblyType)));
    }
}