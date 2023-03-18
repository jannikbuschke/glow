using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Glow.AspNetCore.Utils.Module;
using Glow.Authentication.Aad;
using Glow.Clocks;
using Glow.Glue.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;

namespace Glow.AuditLog
{
    public static class AuditLogStartupExtension
    {
        public static void AddAuditLog<T>(this IServiceCollection services, Options options = null) where
            T : class, IAuditLog
        {
            services.AddScoped<IAuditLog>(provider => provider.GetService<T>());
            services.AddScoped<AuditLogService>();
            services.AddSingleton(
                options ?? new Options() { SystemDisplayName = "System", SystemUserId = "___system___" });
        }
    }

    public class Options
    {
        public string SystemUserId { get; set; }
        public string SystemDisplayName { get; set; }
    }

    public abstract class AuditLogController<T> where T : IEntity
    {
        private readonly IAuditLog log;

        public AuditLogController(IAuditLog log)
        {
            this.log = log;
        }

        [HttpGet("{entityId}")]
        public Task<List<AuditItem>> Get(string entityId)
        {
            return log.AuditItem
                .Where(v => v.EntityId == entityId && v.EntityType == typeof(T).FullName)
                .ToListAsync();
        }
    }

    public class AuditLogService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IAuditLog log;
        private readonly IClock clock;
        private readonly Options options;

        public AuditLogService(IHttpContextAccessor httpContextAccessor, IAuditLog log, IClock clock, Options options)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.log = log;
            this.clock = clock;
            this.options = options;
        }

        public void Log<T>(T entity, AuditItemDefaultOperations operation, bool includeChanges = false,
            bool noLogIfNoChanges = false) where T : IEntity
        {
            Log<T>(entity, operation.ToString(), includeChanges, noLogIfNoChanges);
        }

        public void Log<T>(T entity, string messageType, bool includeChanges = false, bool noLogIfNoChanges = false)
            where T : IEntity
        {
            string entityId = entity.Id.ToString();
            if (string.IsNullOrEmpty(entityId))
            {
                throw new BadRequestException("Entity needs to have an id set");
            }

            ClaimsPrincipal user = httpContextAccessor.HttpContext?.User;
            string userId = user?.GetObjectId() ?? options.SystemUserId;
            string userName = user.Name() ?? options.SystemDisplayName;
            AuditItem auditItem = new()
            {
                CreatedAt = clock.Now,
                CreatedBy = userId,
                CreatedByDisplayName = userName,
                EntityId = entityId,
                EntityType = typeof(T).FullName,
                EntityDisplayName = entity.DisplayName,
                OperationType = messageType
            };

            List<EntityEntry> entries = log.ChangeTracker
                .Entries()
                .Where(v => v.State != EntityState.Detached && v.State != EntityState.Unchanged)
                .ToList();
            if (includeChanges)
            {
                var changes = entries.Select(v => new Change
                {
                    State = v.State,
                    FullTypeName = v.Entity.GetType().FullName,
                    Key = (v.Entity as IEntity)?.Id.ToString()
                          ?? v.Metadata.FindPrimaryKey().Properties.Select(p => v.Property(p.Name).CurrentValue).First()
                              .ToString(),
                    DisplayName = (v.Entity as IEntity)?.DisplayName ?? (v.Entity as IAuditLogDisplayName)?.DisplayName,
                    ChangedProperties = v.OriginalValues.Properties.Select(p =>
                        {
                            var originalValue = v.OriginalValues[p]?.ToString();
                            var newValue = v.CurrentValues[p]?.ToString();
                            var changed = originalValue != newValue;

                            return new ChangedProperty
                            {
                                PropertyName = p.Name,
                                OriginalValue = originalValue,
                                NewValue = newValue,
                                Changed = changed
                            };
                        }).Where(v => v.Changed)
                        .ToList()
                }).ToList();
                auditItem.Changes = changes;
                if (noLogIfNoChanges && changes.Count == 0)
                {
                    // do not log
                }
                else
                {
                    log.AuditItem.Add(auditItem);
                }
            }
            else
            {
                log.AuditItem.Add(auditItem);
            }
        }

        public class Change
        {
            public string FullTypeName { get; set; }
            public EntityState State { get; set; }
            public string Key { get; set; }
            public string DisplayName { get; set; }
            public List<ChangedProperty> ChangedProperties { get; set; }
        }

        public class ChangedProperty
        {
            public string PropertyName { get; set; }
            public string OriginalValue { get; set; }
            public string NewValue { get; set; }
            public bool Changed { get; set; }
        }

        public IQueryable<AuditItem> QueryItemsFor<T>(T entity) where T : IEntity
        {
            string id = entity.Id.ToString();
            string? entityTypeName = typeof(T).FullName;
            return log.AuditItem
                .Where(v => v.EntityId == id && v.EntityType == entityTypeName)
                .OrderByDescending(v => v.CreatedAt);
        }
    }

    public interface IAuditLog
    {
        DbSet<AuditItem> AuditItem { get; }
        ChangeTracker ChangeTracker { get; }
    }

    public enum AuditItemDefaultOperations
    {
        Created = 1,
        Updated = 2,
        Deleted = 3
    }

    public enum Priority
    {
        Low = 1,
        Normal = 2,
        High = 3,
    }

    public class AuditItem
    {
        public Guid Id { get; set; }

        public string CreatedBy { get; set; }
        public string CreatedByDisplayName { get; set; }
        public DateTime CreatedAt { get; set; }

        // Primary key of entity
        public string EntityId { get; set; }

        public string EntityDisplayName { get; set; }

        // Full type Name (including namespace)
        public string EntityType { get; set; }

        /// <summary>
        /// Examples: CircularResolution.InviteSent, CircularResolution.Created, Created
        /// </summary>
        public string OperationType { get; set; }

        public List<AuditLogService.Change> Changes { get; set; }

        // public string MessageTe mplate { get; set; }
        // public IDictionary<string, EntityLink> EntityLinks { get; set; }
    }

    public class UpdatedValue
    {
        /// <summary>
        /// Created, Updated
        /// </summary>
        public string OperationType { get; set; }

        public string EntityType { get; set; }
        public string PropertyName { get; set; }
        public string OldValueDisplayName { get; set; }
        public string NewValueDisplayName { get; set; }
    }

    public class EntityLink
    {
        public string LocalId { get; set; }
        public string Id { get; set; }
        public string EntityType { get; set; }
        public string DisplayName { get; set; }
    }
}
