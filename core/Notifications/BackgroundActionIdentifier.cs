using System;

namespace Glow.Core.Notifications
{
    public class BackgroundActionIdentifier<T>
    {
        public string ActionName { get; }
        public Guid Id { get; set; } = Guid.NewGuid();

        public string FullId()
        {
            return $"{ActionName}:{Id.ToString()}";
        }

        public BackgroundActionIdentifier()
        {
            ActionName = typeof(T).FullName;
        }
    }
}