using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Glow.Authentication.Aad;
using Glow.Core.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Graph;
using Microsoft.Identity.Client;

namespace Glow.MsGraph.Mails
{
    public class MailService
    {
        private readonly IGraphTokenService tokenService;
        private readonly TokenService service;

        public MailService(
            IGraphTokenService tokenService,
            IHttpContextAccessor httpContextAccessor
        )
        {
            this.tokenService = tokenService;
            this.service = service;
        }

        public async Task<Message> Send(Message mail, string mailboxOrUserId = null, string scope = "profile")
        {
            GraphServiceClient client = await tokenService.GetClientForUser(new string[] { scope });
            // AuthenticationResult token = await service.GetAccessTokenAsync(httpContextAccessor.HttpContext.User);
            //
            // var client = new GraphServiceClient(
            //     "https://graph.microsoft.com/v1.0/",
            //     new DelegateAuthenticationProvider(
            //         (requestMessage) =>
            //         {
            //             requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
            //             return Task.CompletedTask;
            //         }
            //     ));

            // User me = await client.Me.Request().GetAsync();

            IUserRequestBuilder userRequestBuilder = string.IsNullOrWhiteSpace(mailboxOrUserId)
                ? client.Me
                : client.Users[mailboxOrUserId];

            Message draft = await userRequestBuilder
                .MailFolders
                .Drafts
                .Messages
                .Request(new List<HeaderOption>() { new HeaderOption("Prefer", "IdType=\"ImmutableId\"") })
                .AddAsync(mail);

            await userRequestBuilder.Messages[draft.Id].Send().Request().PostAsync();

            Message msg = await userRequestBuilder.Messages[draft.Id].Request().GetAsync();

            return msg;
        }
    }
}