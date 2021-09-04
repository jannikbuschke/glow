using System;
using AutoMapper;
using Glow.Core.Actions;
using Glow.CrudHelpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TemplateName.Example;

namespace TemplateName.Example
{
    public class Person
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    [Action(Route = "api/person/create")]
    public class CreatePerson : BaseCreate<Person>
    {
        public string Name { get; set; }
    }

    public class CreatePersonHandler : SimpleCreateHandler<DataContext, CreatePerson, Person>
    {
        public CreatePersonHandler(IServiceProvider services) : base(services) { }
    }

    [Action(Route = "api/person/update")]
    public class UpdatePerson : BaseUpdate<Person>
    {
        public string Name { get; set; }
    }

    public class UpdatePersonHandler : SimpleUpdateHandler<DataContext, UpdatePerson, Person>
    {
        public UpdatePersonHandler(IServiceProvider services) : base(services) { }
    }

    [Action(Route = "api/person/get-list")]
    public class GetPersonList : BaseGetList<Person>
    {
    }

    public class GetPersonListHandler : SimpleGetListHandler<DataContext, GetPersonList, Person>
    {
        public GetPersonListHandler(IServiceProvider services) : base(services) { }
    }

    [Action(Route = "api/person/get-single")]
    public class GetPerson : BaseGetSingle<Person>
    {
    }

    public class GetPersonHandler : SimpleGetSingleHandler<DataContext, GetPerson, Person>
    {
        public GetPersonHandler(IServiceProvider services) : base(services) { }
    }

    [Action(Route = "api/person/delete")]
    public class DeletePerson : BaseDelete<Person>
    {
    }

    public class DeletePersonHandler : SimpleDeleteHandler<DataContext, DeletePerson, Person>
    {
        public DeletePersonHandler(IServiceProvider services) : base(services) { }
    }

    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            CreateMap<CreatePerson, Person>();
            CreateMap<UpdatePerson, Person>();
        }
    }
}

namespace TemplateName
{
    public partial class DataContext
    {
        public DbSet<Person> Person { get; set; }
    }
}