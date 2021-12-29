using System;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Glow.Azdo.Authentication;
using Glow.Core.Actions;
using Glow.Glue.AspNetCore;
using MediatR;
using Microsoft.TeamFoundation.Core.WebApi;

namespace Glow.Sample.Azdo;

[Action(Route = "azdo/create-commit", AllowAnonymous = true)]
public record CreatePullRequest(
    string ProjectId,
    string Path,
    string Content,
    string Name,
    string Description) : IRequest<GitPush>;

public class CreateCommitHandler : BaseHandler,
                                   IRequestHandler<CreatePullRequest, GitPush>
{
    public CreateCommitHandler(AzdoClients clients) : base(clients)
    {
    }

    public async Task<GitPush> Handle(CreatePullRequest request, CancellationToken ct)
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

        var defaultBranchName = repo.DefaultBranch.Remove(0, "refs/".Length);

        var defaultBranch = gitClient.GetRefsAsync(repo.Id, filter: defaultBranchName).Result.First();

        // next, craft the branch and commit that we'll push
        // Name = branchname
        var newBranch = new GitRefUpdate() { Name = $"refs/heads/{Guid.NewGuid().ToString()}", OldObjectId = defaultBranch.ObjectId, };
        var newCommit = new GitCommitRef()
        {
            Comment = request.Description,
            Changes = new GitChange[]
            {
                new()
                {
                    ChangeType = VersionControlChangeType.Edit,
                    Item = new GitItem() { Path = request.Path },
                    NewContent = new ItemContent() { Content = request.Content, ContentType = ItemContentType.RawText, },
                }
            },
        };
        var push = await gitClient.CreatePushAsync(new GitPush() { RefUpdates = new[] { newBranch }, Commits = new[] { newCommit }, }, repo.Id);
        var commit = push.Commits.Last();
        await gitClient.CreatePullRequestAsync(
            new GitPullRequest()
            {
                SourceRefName = newBranch.Name,
                TargetRefName = repo.DefaultBranch,
                Description = request.Description,
                Title = request.Name,
                Commits = new[] { commit }
            }, project.Id,
            repo.Id);
        return push;
    }
}