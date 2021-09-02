namespace Glow.NotificationsCore
{
    public class EntryModified : IMessage<EntryModifiedPayload>
    {
        public string Kind
        {
            get { return nameof(EntryModified); }
        }

        public EntryModifiedPayload Payload { get; set; }
    }
}