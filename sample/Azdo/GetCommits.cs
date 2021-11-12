using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Glow.Azdo.Authentication;
using Glow.Core.Actions;
using Glow.Glue.AspNetCore;
using MediatR;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;

namespace Glow.Sample.Azdo;

[Action(Route = "azdo/get-commits", AllowAnoymous = true)]
public record GetCommits(string ProjectName) : IRequest<IEnumerable<Commit>>;

[Action(Route = "azdo/get-items", AllowAnoymous = true)]
public record GetItems(string ProjectId) : IRequest<List<GitItem>>;

[Action(Route = "azdo/get-item", AllowAnoymous = true)]
public record GetItem(string ProjectId, string Path) : IRequest<StringWrapper>;

public record StringWrapper(string Value);

public class Commit
{
    public GitCommit GitCommit { get; set; }
    public GitCommitRef GitCommitRef { get; set; }
    public GitCommitChanges Changes { get; set; }
}

public class GetCommitsHandler : BaseHandler,
                                 IRequestHandler<GetCommits, IEnumerable<Commit>>,
                                 IRequestHandler<GetItem, StringWrapper>,
                                 IRequestHandler<GetItems, List<GitItem>>
{
    public GetCommitsHandler(AzdoClients clients) : base(clients)
    {
    }

    public async Task<IEnumerable<Commit>> Handle(GetCommits request, CancellationToken ct)
    {
        var projectClient = await clients.GetAppClient<ProjectHttpClient>();
        var projects = await projectClient.GetProjects();
        var project = projects.FirstOrDefault(v => v.Name == request.ProjectName);
        if (project == null)
        {
            throw new BadRequestException($"project {request.ProjectName} not found");
        }

        var gitClient = await clients.GetAppClient<GitHttpClient>();
        var repositories = await gitClient.GetRepositoriesAsync(project.Id);
        var repo = repositories.FirstOrDefault();
        if (repo == null)
        {
            throw new BadRequestException($"No repo in project {request.ProjectName}");
        }

        var commits = await gitClient.GetCommitsAsync(project.Id, repo.Id, new GitQueryCommitsCriteria());
        var result = new List<Commit>();
        foreach (var v in commits)
        {
            var changes = await gitClient.GetChangesAsync(project.Id, v.CommitId, repo.Id);
            var commit = await gitClient.GetCommitAsync(project.Id, v.CommitId, repo.Id);

            result.Add(new Commit() { GitCommit = commit, GitCommitRef = v, Changes = changes });
        }

        return result;
    }

    public async Task<StringWrapper> Handle(GetItem request, CancellationToken cancellationToken)
    {
        var projectClient = await clients.GetAppClient<ProjectHttpClient>();
        var project = await projectClient.GetProject(request.ProjectId);
        if (project == null)
        {
            throw new BadRequestException($"project {request.ProjectId} not found");
        }

        var gitClient = await clients.GetAppClient<GitHttpClient>();
        var repositories = await gitClient.GetRepositoriesAsync(project.Id);
        var repo = repositories.FirstOrDefault();
        if (repo == null)
        {
            throw new BadRequestException($"No repo in project {request.ProjectId}");
        }

        var content = await gitClient.GetItemContentAsync(project.Id, repo.Id, request.Path);
        var reader = new StreamReader(content);
        var text = await reader.ReadToEndAsync();
        return new StringWrapper(text);
    }

    public async Task<List<GitItem>> Handle(GetItems request, CancellationToken cancellationToken)
    {
        var projectClient = await clients.GetAppClient<ProjectHttpClient>();
        var project = await projectClient.GetProject(request.ProjectId);
        if (project == null)
        {
            throw new BadRequestException($"project {request.ProjectId} not found");
        }

        var gitClient = await clients.GetAppClient<GitHttpClient>();
        var repositories = await gitClient.GetRepositoriesAsync(project.Id);
        var repo = repositories.FirstOrDefault();
        if (repo == null)
        {
            throw new BadRequestException($"No repo in project {request.ProjectId}");
        }

        var items0 = await gitClient.GetItemsPagedAsync(project.Id, repo.Id, "/");

        var items = await gitClient.GetItemsAsync(project.Id, repo.Id, scopePath: "/");
        return items0.ToList();
    }
}