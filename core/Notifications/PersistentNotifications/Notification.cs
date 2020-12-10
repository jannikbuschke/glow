using System;

namespace Glow.NotificationsCore
{
    public abstract class Notification
    {
        public Guid Id { get; set; }
        public string Message { get; set; }
        public string UserId { get; set; }
        public string Link { get; set; }
        public bool HasBeenRead { get; set; }
    }
}
