using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.EquivalencyExpression;
using MediatR;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JannikB.Glue;
using App.Data;
using Glow.Core.FakeData;
using App.$1s;

namespace App.$1s
{
    public class $1Profile : Profile
    {
        public $1Profile()
        {
            CreateMap<Update$1, $1>().EqualityComparison((v1, v2) => v1.Id == v2.Id);
            CreateMap<Create$1, $1>().EqualityComparison((v1, v2) => v1.Id == v2.Id);
        }
    }

    public class Update$1 : IRequest<$1>
    {
        public Guid Id { get; set; }
    }

    public class Create$1 : IRequest<$1>
    {
    }

    public class $1
    {
        public Guid Id { get; set; }
    }

    public class Update$1Handler
        : IRequestHandler<Create$1, $1>
        , IRequestHandler<Update$1, $1>
    {
        private readonly DataContext ctx;
        private readonly IMapper mapper;

        public Update$1Handler(DataContext ctx, IMapper mapper)
        {
            this.ctx = ctx;
            this.mapper = mapper;
        }

        public async Task<$1> Handle(Create$1 request, CancellationToken cancellationToken)
        {
            $1 entity = mapper.Map(request, new $1());
            ctx.$1.Add(entity);
            await ctx.SaveChangesAsync();
            return entity;
        }

        public async Task<$1> Handle(Update$1 request, CancellationToken cancellationToken)
        {
            $1 entity = await ctx.$1.FindAsync(request.Id);
            mapper.Map(request, entity);
            await ctx.SaveChangesAsync();
            return entity;
        }
    }

    public class $1SampleData : FakeBase<$1>
    {
        public $1SampleData() : base(f =>
            {
                return f
                .RuleFor(v => v.Id, f => Guid.NewGuid())
                .Generate(10);
            })
        {
        }
    }

    [Route("api/${1/(.*)/${1:/downcase}/}")]
    [ApiController]
    public class $1Controller : ControllerBase
    {
        private readonly DataContext ctx;
        private readonly IMediator mediator;

        public $1Controller(DataContext ctx, IMediator mediator)
        {
            this.ctx = ctx;
            this.mediator = mediator;
        }

        [Validatable]
        [HttpPost("create")]
        public async Task<ActionResult<$1>> Create(Create$1 request)
        {
            var result = await mediator.Send(request);
            return result;
        }

        [Validatable]
        [HttpPost("update")]
        public async Task<ActionResult<$1>> Update(Update$1 request)
        {
            var result = await mediator.Send(request);
            return result;
        }

        [HttpGet("list")]
        public IQueryable<$1> List() {
            return ctx.$1;
        }
    }
}

namespace App.Data {
    public partial class DataContext {
        public DbSet<$1> $1 { get; set; }
    }
}
