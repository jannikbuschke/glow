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
            CreateMap<CreateMdx, Mdx>();
            CreateMap<UpdateMdx, Mdx>();
        }
    }

    public class Mdx
    {
        public Guid Id { get; set; }
        public string Path { get; set; }
        public string Content { get; set; }
    }

    public class MdxViewmodel
    {
        public Guid Id { get; set; }
        public string Path { get; set; }
        public string Content { get; set; }
        public string Code { get; set; }
        public string Error { get; set; }
        public Dictionary<string, string> Frontmatter { get; set; }
    }

    [Action(Route = "api/mdx/create", Policy = Policies.Authorized)]
    public class CreateMdx : BaseCreate<Mdx>
    {
        public string Path { get; set; }
        public string Content { get; set; }
    }

    public class CreateMdxHandler : SimpleCreateHandler<DataContext, CreateMdx, Mdx>
    {
        public CreateMdxHandler(IServiceProvider services) : base(services) { }
    }

    [Action(Route = "api/mdx/update", Policy = Policies.Authorized)]
    public class UpdateMdx : BaseUpdate<Mdx>
    {
        public string Path { get; set; }
        public string Content { get; set; }
    }

    public class UpdateMdxHandler : SimpleUpdateHandler<DataContext, UpdateMdx, Mdx>
    {
        public UpdateMdxHandler(IServiceProvider services) : base(services) { }
    }

    [Action(Route = "api/mdx/get-list", Policy = Policies.Authorized)]
    public class GetMdxList : BaseGetList<Mdx>
    {
    }

    public class GetMdxListHandler : SimpleGetListHandler<DataContext, GetMdxList, Mdx>
    {
        public GetMdxListHandler(IServiceProvider ctx) : base(ctx) { }
    }

    [Action(Route = "api/mdx/get-single", Policy = Policies.Authorized)]
    public class GetEntityViewmodel : BaseGetSingle<MdxViewmodel>
    {
    }

    public class GetEntityViewmodelHandler : IRequestHandler<GetEntityViewmodel, MdxViewmodel>
    {
        private readonly DataContext ctx;
        private readonly INodeJSService node;
        private readonly ILogger<GetEntityViewmodelHandler> logger;
        private readonly IMediator mediator;

        public GetEntityViewmodelHandler(
            DataContext ctx,
            INodeJSService node,
            ILogger<GetEntityViewmodelHandler> logger,
            IMediator mediator
        )
        {
            this.ctx = ctx;
            this.node = node;
            this.logger = logger;
            this.mediator = mediator;
        }

        public class Data
        {
            public string Code { get; set; }
            public Dictionary<string, string> FrontMatter { get; set; }
            public object Files { get; set; }
            public string Formatted { get; set; }
        }

        public async Task<MdxViewmodel> Handle(
            GetEntityViewmodel request,
            CancellationToken cancellationToken)
        {
            var entity = await ctx.MdxBundle.SingleAsync(v => v.Id == request.Id);

            try
            {
                var result = await mediator.Send(new Transpile() { Source = entity.Content });

                return new MdxViewmodel()
                {
                    Id = entity.Id,
                    Code = result.Code,
                    //Code = result1.Code,
                    // Code = result1.GetProperty("code").GetString(),
                    Content = entity.Content,
                    Path = entity.Path,
                    Frontmatter = result.Frontmatter
                };
            }
            catch (Exception e)
            {
                return new MdxViewmodel() { Error = e.Message };
            }
        }

        public class TranspileResult
        {
            public string Code { get; set; }
            public Dictionary<string, string> Frontmatter { get; set; }
        }

        [Action(Route = "api/mdx/transpile", Policy = Policies.Authorized)]
        public class Transpile : IRequest<TranspileResult>
        {
            public string Source { get; set; }
        }

        public class Transpilehandler : IRequestHandler<Transpile, TranspileResult>
        {
            private readonly INodeJSService node;
            private readonly DataContext ctx;

            public Transpilehandler(INodeJSService node, DataContext ctx)
            {
                this.node = node;
                this.ctx = ctx;
            }

            public async Task<TranspileResult> Handle(Transpile request, CancellationToken cancellationToken)
            {
                if (request.Source == null)
                {
                    return new TranspileResult() { Code = "", Frontmatter = new Dictionary<string, string>() };
                }
                var files = await ctx.MdxBundle.ToListAsync();
                Data result1 = await node
                    .InvokeFromFileAsync<Data>("./transpile-react.js", args: new object[] { request.Source, files })
                    .ConfigureAwait(false);
                return new TranspileResult() { Code = result1.Code, Frontmatter = result1.FrontMatter };
            }
        }
    }
}