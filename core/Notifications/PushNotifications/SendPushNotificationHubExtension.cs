using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Glow.NotificationsCore
{
    public static class SendPushNotificationHubExtension
    {
        public static async Task SendPushNotification<THub>(
            this IHubContext<THub> hub,
            string userId,
            PushNotification notification
        ) where THub : Hub
        {
            await hub.Clients.User(userId).SendAsync("push-notification", notification);
        }
    }
}
