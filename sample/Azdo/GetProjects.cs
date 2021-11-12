using System.Threading;
using System.Threading.Tasks;
using Glow.Azdo.Authentication;
using Glow.Core.Actions;
using MediatR;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.WebApi;

namespace Glow.Sample.Azdo;

[Action(Route = "azdo/get-projects", AllowAnoymous = true)]
public record GetProjects : IRequest<IPagedList<TeamProjectReference>>;

public class GetProjectsHandler : BaseHandler,
                                  IRequestHandler<GetProjects, IPagedList<TeamProjectReference>>
{
    public GetProjectsHandler(AzdoClients clients) : base(clients) { }

    public async Task<IPagedList<TeamProjectReference>> Handle(GetProjects request, CancellationToken ct)
    {
        var projectClient = await clients.GetAppClient<ProjectHttpClient>();
        var projects = await projectClient.GetProjects();
        return projects;
    }
}