extern alias GraphBeta;
using System;
using Beta = GraphBeta.Microsoft.Graph;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Glow.Authentication.Aad;
using Glow.Core.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Identity.Client;

namespace Glow.MsGraph.Mails
{
    public static class BetaExtensions
    {
        private static Func<Beta.Recipient, Recipient> ToV1Recipient = (x)=>
         new Recipient()
            {
                EmailAddress = new() { Address = x.EmailAddress.Address, Name = x.EmailAddress?.Name }
            };

        private static Func<Recipient, Beta.Recipient> ToBetaRecipient = (x)=>
            new Beta.Recipient()
            {
                EmailAddress = new() { Address = x.EmailAddress.Address, Name = x.EmailAddress?.Name }
            };

        public static Beta.Message FromV1Message(Message message)
        {
            var attachments = new Beta.MessageAttachmentsCollectionPage();
            // other attachment types currently not supported
            IEnumerable<FileAttachment> fileAttachments = message.Attachments != null ? message.Attachments.Cast<FileAttachment>():new List<FileAttachment>();
            IEnumerable<Beta.FileAttachment> betaFileAttachments = fileAttachments.Select(v => new Beta.FileAttachment() { Id = v.Id, ContentBytes = v.ContentBytes });
            var betaAttachments = new Beta.MessageAttachmentsCollectionPage();
            return new()
            {
                Id = message.Id,
                Subject = message.Subject,
                Sender = ToBetaRecipient.Invoke(message.Sender),
                From = ToBetaRecipient.Invoke(message.From),
                Body = new()
                {
                    Content = message.Body.Content, ContentType = message.Body.ContentType == BodyType.Text ? Beta.BodyType.Text : Beta.BodyType.Html
                },
                IsDraft = message.IsDraft,
                Attachments = new Beta.MessageAttachmentsCollectionPage(),
                ToRecipients = message.ToRecipients.Select(ToBetaRecipient),
                CcRecipients = message.CcRecipients.Select(ToBetaRecipient),
                BccRecipients = message.BccRecipients.Select(ToBetaRecipient),
                SentDateTime = message.SentDateTime,
            };
        }

        public static Message ToV1Message(Beta.Message message)
        {
            return new Message
            {
                Id = message.Id, Subject = message.Subject,
                Sender = ToV1Recipient.Invoke(message.Sender),
                From = ToV1Recipient.Invoke(message.From),
                Body= new(){Content = message.Body.Content,ContentType = message.Body.ContentType==Beta.BodyType.Text?BodyType.Text:BodyType.Html},
                ToRecipients = message.ToRecipients.Select(ToV1Recipient),
                CcRecipients = message.CcRecipients.Select(ToV1Recipient),
                BccRecipients = message.BccRecipients.Select(ToV1Recipient),
                IsDraft = message.IsDraft,
                SentDateTime = message.SentDateTime,
            };
        }
    }

    /// <summary>
    /// Uses a microsoft graph mailbox and user delegated permission
    /// </summary>
    public class MailService
    {
        private readonly IGraphTokenService tokenService;
        private readonly ILogger<MailService> logger;
        private readonly TokenService service;

        public MailService(
            IGraphTokenService tokenService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<MailService> logger
        )
        {
            this.tokenService = tokenService;
            this.logger = logger;
        }

        public async Task<Beta.Message> SendBeta(Beta.Message mail, string mailboxOrUserId = null, string scope = "profile")
        {
            Beta.GraphServiceClient client = await tokenService.GetBetaClientForUser(new string[] { scope });

            Beta.IUserRequestBuilder userRequestBuilder = string.IsNullOrWhiteSpace(mailboxOrUserId)
                ? client.Me
                : client.Users[mailboxOrUserId];

            Beta.IMessageAttachmentsCollectionPage attachments = mail.Attachments;
            GraphResponse<Beta.Message> response = await userRequestBuilder
                .MailFolders
                .Drafts
                .Messages
                .Request(new List<HeaderOption>() { new HeaderOption("Prefer", "IdType=\"ImmutableId\"") })
                .AddResponseAsync(mail);

            Beta.Message draft = await response.GetResponseObjectAsync();

            await userRequestBuilder.Messages[draft.Id].Send().Request().PostAsync();

            Beta.Message msg = await userRequestBuilder.Messages[draft.Id].Request().GetAsync();

            return msg;
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