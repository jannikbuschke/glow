using System;
using System.ComponentModel.DataAnnotations;

namespace Glow.NotificationsCore
{
    public class MarkAsRead
    {
        [Required]
        public Guid? NotificationId { get; set; }
    }
}
