using Microsoft.Extensions.DependencyInjection;

namespace Glow.Azure.AzureKeyVault
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddAzureKeyvaultClientProvider(this IServiceCollection collection)
        {
            return collection.AddTransient<AzureKeyvaultClientProvider>();
        }
    }
}