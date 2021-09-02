using System;
using Gertrud.Users;
using Glow.Clocks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;

namespace Glow.AspNetCore.Utils.Module
{
    public interface IEntity : IEntity<Guid>
    {
    }

    public interface IEntity<T>
    {
        T Id { get; }

        string DisplayName { get; set; }
    }

    public interface IAuditLogDisplayName
    {
        string DisplayName { get; }
    }

    public interface ITrackEdits
    {
        string CreatedBy { get; set; }
        DateTime? CreatedAt { get; set; }
        string UpdatedBy { get; set; }
        DateTime? UpdatedAt { get; set; }
    }

    public static class DataContextExtension
    {
        public static void HandleITrackEdits(this DbContext self, IServiceProvider services,
            string systemUserId = User.SystemUserId)
        {
            IHttpContextAccessor httpContextAccessor = services.GetRequiredService<IHttpContextAccessor>();
            var userId = httpContextAccessor?.HttpContext?.GetUserObjectId() ?? systemUserId;
            IClock clock = services.GetRequiredService<IClock>();

            foreach (EntityEntry<ITrackEdits> v in self.ChangeTracker.Entries<ITrackEdits>())
            {
                if (v.State == EntityState.Added)
                {
                    v.Entity.CreatedAt ??= clock.Now;
                    v.Entity.CreatedBy ??= userId;
                }

                if (v.State == EntityState.Modified)
                {
                    v.Entity.UpdatedAt = clock.Now;
                    v.Entity.UpdatedBy = userId;
                }
            }
        }
    }
}