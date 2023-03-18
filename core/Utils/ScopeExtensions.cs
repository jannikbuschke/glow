using Microsoft.Extensions.DependencyInjection;

namespace Glow.Core.Utils;

public static class ScopeExtensions
{
    public static T Get<T>(this IServiceScope scope)
    {
        return scope.ServiceProvider.GetRequiredService<T>();
    }
    public static T GetService<T>(this IServiceScope scope)
    {
        return scope.ServiceProvider.GetService<T>();
    }

    public static T GetRequiredService<T>(this IServiceScope scope)
    {
        return scope.ServiceProvider.GetRequiredService<T>();
    }
}
