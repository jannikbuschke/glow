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
    /// <summary>
    /// Uses a microsoft graph mailbox and user delegated permission
    /// </summary>
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
            // use scopes: mail.send, mail.readwrite.shared
            GraphServiceClient client = await tokenService.GetClientForUser(new string[] { scope });

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