using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Glow.Azdo.Authentication;
using Glow.Core.Actions;
using Glow.Glue.AspNetCore;
using MediatR;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;

namespace Glow.Sample.Azdo;

[Action(Route = "azdo/create-library", AllowAnonymous = true)]
public record CreateLibrary(string ProjectName) : IRequest<VariableGroup>;

public class CreateLibraryResult
{
}

public class CreateLibraryHandler : IRequestHandler<CreateLibrary, VariableGroup>
{
    private readonly AzdoClients clients;

    public CreateLibraryHandler(AzdoClients clients)
    {
        this.clients = clients;
    }

    public async Task<VariableGroup> Handle(CreateLibrary request, CancellationToken ct)
    {
        // var client = await clients.GetAppClient<WorkItemTrackingHttpClient>();
        // var client2 = await clients.GetAppClient< Microsoft.TeamFoundation.Build.WebApi.BuildHttpClient>();
        // var client3 = await clients.GetAppClient< ProjectHttpClient>();
        var client4 = await clients.GetAppClient<Microsoft.TeamFoundation.DistributedTask.WebApi.TaskAgentHttpClient>();
        var result = await client4.AddVariableGroupAsync(request.ProjectName,
            new VariableGroupParameters()
            {
                Description = "created automatically",
                Name = "customer-xy",
                Variables = new Dictionary<string, VariableValue> { { "key1", new VariableValue("value", isSecret: false) } }
            }, null, ct);

        var gitClient = await clients.GetAppClient<GitHttpClient>();

        ProjectHttpClient projectClient = await clients.GetAppClient<ProjectHttpClient>();
        var projects = await projectClient.GetProjects();
        var project = projects.FirstOrDefault(v => v.Name == request.ProjectName);
        if (project == null)
        {
            throw new BadRequestException($"project {request.ProjectName} not found");
        }

        var repositories = await gitClient.GetRepositoriesAsync(project.Id);
        var repo = repositories.FirstOrDefault();
        if (repo == null)
        {
            throw new BadRequestException($"No repo in project {request.ProjectName}");
        }

        var defaultBranchName = repo.DefaultBranch.Remove(0, "refs/".Length);

        GitRef defaultBranch = gitClient.GetRefsAsync(repo.Id, filter: defaultBranchName).Result.First();


        // // next, craft the branch and commit that we'll push
        // GitRefUpdate newBranch = new GitRefUpdate() { Name = $"refs/heads/vsts-api-sample/test-branch-name", OldObjectId = defaultBranch.ObjectId, };
        // string newFileName = $"{GitSampleHelpers.ChooseItemsafeName()}.md";
        // GitCommitRef newCommit = new GitCommitRef()
        // {
        //     Comment = "Add a sample file",
        //     Changes = new GitChange[]
        //     {
        //         new GitChange()
        //         {
        //             ChangeType = VersionControlChangeType.Add,
        //             Item = new GitItem() { Path = $"/vsts-api-sample/{newFileName}" },
        //             NewContent = new ItemContent() { Content = "# Thank you for using VSTS!", ContentType = ItemContentType.RawText, },
        //         }
        //     },
        // };
        //
        // // create the push with the new branch and commit
        // GitPush push = gitClient
        //     .CreatePushAsync(new GitPush() { RefUpdates = new GitRefUpdate[] { newBranch }, Commits = new GitCommitRef[] { newCommit }, }, repo.Id).Result;
        //
        // Console.WriteLine("project {0}, repo {1}", project.Name, repo.Name);
        // Console.WriteLine("push {0} updated {1} to {2}",
        //     push.PushId, push.RefUpdates.First().Name, push.Commits.First().CommitId);
        //
        // // now clean up after ourselves (and in case logging is on, don't log these calls)
        // ClientSampleHttpLogger.SetSuppressOutput(this.Context, true);
        //
        // // delete the branch
        // GitRefUpdateResult refDeleteResult = gitClient.UpdateRefsAsync(
        //     new GitRefUpdate[]
        //     {
        //         new GitRefUpdate()
        //         {
        //             OldObjectId = push.RefUpdates.First().NewObjectId, NewObjectId = new string('0', 40), Name = push.RefUpdates.First().Name,
        //         }
        //     },
        //     repositoryId: repo.Id).Result.First();
        //
        // // pushes and commits are immutable, so no way to clean them up
        // // but the commit will be unreachable after this
        //
        // return push;
        return result;
    }
}

