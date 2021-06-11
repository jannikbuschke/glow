using System.Threading.Tasks;
using Glow.NotificationsCore;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace Glow.Core.Notifications
{
    public static class ServiceExtensions
    {
        public static void AddGlowNotifications<T>(this IServiceCollection services) where T : Hub
        {
            services.AddScoped<IClientNotificationService, ClientNotificationService<T>>();
        }
    }

    public class ClientNotificationService<THub> : IClientNotificationService where THub : Hub
    {
        private readonly IHubContext<THub> hubContext;

        public ClientNotificationService(IHubContext<THub> hubContext)
        {
            this.hubContext = hubContext;
        }

        public async Task PublishNotification(IClientNotification notification)
        {
            await hubContext.PublishNotification(notification);
        }
    }

    public interface IClientNotificationService
    {
        Task PublishNotification(IClientNotification notification);
    }
}