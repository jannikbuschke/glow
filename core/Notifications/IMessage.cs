using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Glow.NotificationsCore
{
    public interface IMessage<T>
    {
        string Kind { get; }
        T Payload { get; set; }
    }

    public static class SendMessageHubExtension
    {
        public static async Task SendMessage<THub, T>(
            this IHubContext<THub> hub,
            string userId,
            IMessage<T> message
        ) where THub : Hub
        {
            await hub.Clients.User(userId).SendAsync(
                "message",
                $"{message.GetType().FullName}",
                message
            );
        }

    }
}
