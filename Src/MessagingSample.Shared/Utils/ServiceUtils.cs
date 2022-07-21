using System.Reflection;

namespace MessagingSample.Shared.Utils;

public static class ServiceUtils
{
    public static IEnumerable<Type> GetGenericTypes<TValue>()
    {
        return Assembly.GetExecutingAssembly().GetTypes()
            .Where(type => typeof(TValue).IsAssignableFrom(type) && !type.IsInterface).ToList();
    }
}