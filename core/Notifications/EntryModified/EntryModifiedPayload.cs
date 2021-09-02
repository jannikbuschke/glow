namespace Glow.NotificationsCore
{
    public class EntryModifiedPayload
    {
        public EntryModifiedPayload(string entityName, Operation operation)
        {
            EntityName = entityName;
            Operation = operation;
        }

        public string EntityName { get; private set; }
        public Operation Operation { get; private set; }
    }
}