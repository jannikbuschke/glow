using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Glow.Core.Actions;
using Glow.Glue.AspNetCore;
using Glow.TypeScript;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Glow.CrudHelpers
{
    public abstract class BaseGetSingle<Entity> : IRequest<Entity>
    {
        [Required]
        public Guid? Id { get; set; }
    }

    public abstract class SimpleGetSingleHandler<DataContext, GetSingle, Entity>
        : IRequestHandler<GetSingle, Entity>
        where GetSingle : BaseGetSingle<Entity> where DataContext : DbContext where Entity : class
    {
        private readonly DataContext ctx;

        public SimpleGetSingleHandler(IServiceProvider ctx)
        {
            this.ctx = ctx.GetRequiredService<DataContext>();
        }

        public async Task<Entity> Handle(GetSingle request, CancellationToken cancellationToken)
        {
            Entity entity = await ctx.Set<Entity>().FindAsync(request.Id);
            return entity;
        }
    }

    public abstract class BaseGetList<Entity> : IRequest<IEnumerable<Entity>>
    {
    }

    public abstract class SimpleGetListHandler<DataContext, GetList, Entity>
        : IRequestHandler<GetList, IEnumerable<Entity>>
        where GetList : BaseGetList<Entity> where DataContext : DbContext where Entity : class
    {
        private readonly DataContext ctx;

        public SimpleGetListHandler(IServiceProvider ctx)
        {
            this.ctx = ctx.GetRequiredService<DataContext>();
        }

        public async Task<IEnumerable<Entity>> Handle(GetList request, CancellationToken cancellationToken)
        {
            List<Entity> entities = await ctx.Set<Entity>().ToListAsync();
            return entities;
        }
    }

    public abstract class BaseCreate<Entity> : IRequest<Entity> where Entity : class
    {
    }

    public abstract class SimpleCreateHandler<DataContext, Create, Entity>
        : IRequestHandler<Create, Entity>
        where Create : BaseCreate<Entity> where DataContext : DbContext where Entity : class
    {
        private readonly DataContext ctx;
        private readonly IMapper mapper;

        public SimpleCreateHandler(IServiceProvider services)
        {
            ctx = services.GetRequiredService<DataContext>();
            mapper = services.GetRequiredService<IMapper>();
        }

        public virtual async Task<Entity> Handle(Create request, CancellationToken cancellationToken)
        {
            await Validate(request);
            Entity entity = mapper.Map<Entity>(request);
            ctx.Add(entity);
            await ctx.SaveChangesAsync();
            return entity;
        }

        protected virtual Task Validate(Create request)
        {
            return Task.CompletedTask;
        }
    }

    public abstract class BaseUpdate<Entity> : IRequest<Entity> where Entity : class
    {
        [Required]
        public Guid? Id { get; set; }
    }

    public abstract class SimpleUpdateHandler<DataContext, Update, Entity>
        : IRequestHandler<Update, Entity>
        where Update : BaseUpdate<Entity> where DataContext : DbContext where Entity : class
    {
        private readonly DataContext ctx;
        private readonly IMapper mapper;

        public SimpleUpdateHandler(IServiceProvider services)
        {
            ctx = services.GetRequiredService<DataContext>();
            mapper = services.GetRequiredService<IMapper>();
        }

        public async Task<Entity> Handle(Update request, CancellationToken cancellationToken)
        {
            await Validate(request);
            Entity entity = await ctx.Set<Entity>().FindAsync(request.Id);
            if (entity == null)
            {
                throw new NotFoundException($"Entity '{typeof(Entity).FullName}, {request.Id}' not found");
            }

            mapper.Map(request, entity);
            await ctx.SaveChangesAsync();
            return entity;
        }

        protected virtual Task Validate(Update request)
        {
            return Task.CompletedTask;
        }
    }

    public abstract class BaseDelete<Entity> : IRequest<Entity>
    {
        [Required]
        public Guid? Id { get; set; }
    }

    public abstract class SimpleDeleteHandler<DataContext, Delete, Entity> : IRequestHandler<Delete, Entity>
        where Delete : BaseDelete<Entity> where Entity : class where DataContext : DbContext
    {
        protected readonly DataContext ctx;
        private readonly IMapper mapper;

        public SimpleDeleteHandler(IServiceProvider services)
        {
            ctx = services.GetRequiredService<DataContext>();
            mapper = services.GetRequiredService<IMapper>();
        }

        public async Task<Entity> Handle(Delete request, CancellationToken cancellationToken)
        {
            await Validate(request);
            Entity entity = await ctx.Set<Entity>().FindAsync(request.Id);
            if (entity == null)
            {
                throw new NotFoundException($"Entity '{typeof(Entity).FullName}, {request.Id}' not found");
            }

            ctx.Set<Entity>().Remove(entity);
            await ctx.SaveChangesAsync();
            return entity;
        }

        protected virtual Task Validate(Delete request)
        {
            return Task.CompletedTask;
        }
    }
}