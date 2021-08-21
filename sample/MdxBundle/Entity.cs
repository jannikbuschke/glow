using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Glow.Core.Actions;
using Glow.CrudHelpers;
using Glow.Glue.AspNetCore;
using Jering.Javascript.NodeJS;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Glow.Sample.MdxBundle
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            CreateMap<Create, Entity>();
            CreateMap<Update, Entity>();
        }
    }

    public class Entity
    {
        public Guid Id { get; set; }
        public string Path { get; set; }
        public string Content { get; set; }
    }

    public class EntityViewmodel
    {
        public Guid Id { get; set; }
        public string Path { get; set; }
        public string Content { get; set; }
        public string Code { get; set; }
        public string Error { get; set; }
    }

    [Action(Route = "api/source-file/create", Policy = Policies.Authorized)]
    public class Create : BaseCreate<Entity>
    {
        public string Path { get; set; }
        public string Content { get; set; }
    }

    public class CreateHandler : SimpleCreateHandler<DataContext, Create, Entity>
    {
        public CreateHandler(IServiceProvider services) : base(services) { }
    }

    [Action(Route = "api/source-file/update", Policy = Policies.Authorized)]
    public class Update : BaseUpdate<Entity>
    {
        public string Path { get; set; }
        public string Content { get; set; }
    }

    public class UpdateHandler : SimpleUpdateHandler<DataContext, Update, Entity>
    {
        public UpdateHandler(IServiceProvider services) : base(services) { }
    }

    [Action(Route = "api/source-file/get-list", Policy = Policies.Authorized)]
    public class GetList : BaseGetList<Entity>
    {
    }

    public class GetListHandler : SimpleGetListHandler<DataContext, GetList, Entity>
    {
        public GetListHandler(IServiceProvider ctx) : base(ctx) { }
    }

    [Action(Route = "api/source-file/get-single", Policy = Policies.Authorized)]
    public class GetEntityViewmodel : BaseGetSingle<EntityViewmodel>
    {
    }

    public class GetEntityViewmodelHandler : IRequestHandler<GetEntityViewmodel, EntityViewmodel>
    {
        private readonly DataContext ctx;
        private readonly INodeJSService node;
        private readonly ILogger<GetEntityViewmodelHandler> logger;

        public GetEntityViewmodelHandler(
            DataContext ctx,
            INodeJSService node,
            ILogger<GetEntityViewmodelHandler> logger)
        {
            this.ctx = ctx;
            this.node = node;
            this.logger = logger;
        }

        public class Data
        {
            public string Code { get; set; }
            public object FrontMatter { get; set; }
            public object Files { get; set; }
        }

        public async Task<EntityViewmodel> Handle(
            GetEntityViewmodel request,
            CancellationToken cancellationToken)
        {
            try
            {
                var entity = await ctx.MdxBundle.SingleAsync(v => v.Id == request.Id);

                var files = await ctx.MdxBundle.ToListAsync();

                string code = null;
                string error = null;
                try
                {
                    Data result1 = await node
                        .InvokeFromFileAsync<Data>("./transpile-react.js", args: new object[] {entity.Content, files})
                        .ConfigureAwait(false);
                    code = result1.Code;
                }
                catch (Exception e)
                {
                    error = e.Message;
                }

                return new EntityViewmodel()
                {
                    Id = entity.Id,
                    Code = code,
                    //Code = result1.Code,
                    // Code = result1.GetProperty("code").GetString(),
                    Content = entity.Content,
                    Path = entity.Path,
                    Error = error
                };
            }
            catch (Exception e)
            {
                logger.LogError(e, "error while transpiling");
                throw;
                // throw new BadRequestException("Some error occured");
            }
        }
    }
}