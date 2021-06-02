using System;

namespace Glow.AspNetCore.Utils.Module
{
    public interface IEntity: IEntity<Guid>{}

    public interface IEntity<T>
    {
        T Id { get; }

        public string DisplayName { get; set; }
    }

    public interface IAuditLogDisplayName
    {
        string DisplayName { get; }
    }
}
