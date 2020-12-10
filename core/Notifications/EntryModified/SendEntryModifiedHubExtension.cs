using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Glow.NotificationsCore
{
    public static class SendEntryModifiedHubExtension
    {
        public static async Task SendEntryModified<THub>(
            this IHubContext<THub> hub,
            string userId,
            EntryModified message
        ) where THub : Hub
        {
            await hub.Clients.User(userId).SendAsync(
                "message",
                nameof(EntryModified),
                message
            );
        }
    }
}
