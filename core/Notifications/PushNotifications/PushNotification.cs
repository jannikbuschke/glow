namespace Glow.NotificationsCore
{
    public class PushNotification
    {
        public PushNotification(string message, PushNotificationType type = PushNotificationType.Info)
        {
            Message = message;
            Type = type;
        }

        public string Message { get; }
        public PushNotificationType Type { get; }
        public string Link { get; set; }
    }
}
