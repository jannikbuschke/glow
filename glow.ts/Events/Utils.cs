using System.Reflection;
using Glow.NotificationsCore;

namespace Glow.Ts.Events;

public static class GetEventsExtension
{
    public static IEnumerable<Type> GetEvents(this IEnumerable<Assembly> assemblies)
    {
        IEnumerable<Type> candidates = assemblies
            .SelectMany(v => v.GetExportedTypes().Where(x => x.GetInterfaces().Contains(typeof(IClientNotification))));

        return candidates;
    }
}