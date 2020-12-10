using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Glow.NotificationsCore
{
    public interface INotificationsService
    {
        Task MarkAsRead(Guid notificationId);
        Task<IEnumerable<Notification>> GetUnreadNotifications();
        Task<IEnumerable<Notification>> GetReadNotifications();
    }
}
