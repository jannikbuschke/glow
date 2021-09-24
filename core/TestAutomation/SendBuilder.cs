using System;
using System.Net.Http;
using System.Threading.Tasks;
using Glow.Users;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Glow.Tests
{
    public class SendBuilder<TRequest, TStartup> where TStartup : class
    {
        private readonly WebApplicationFactory<TStartup> factory;
        private readonly object request;
        private string url;
        private UserDto user;

        public SendBuilder(WebApplicationFactory<TStartup> factory, object request)
        {
            this.factory = factory;
            this.request = request;
        }

        public SendBuilder<TRequest, TStartup> To(string url)
        {
            this.url = url;
            return this;
        }

        // public SendBuilder<TRequest, TStartup> As(string userId)
        // {
        //     user = new UserDto { Id = userId };
        //     return this;
        // }

        public SendBuilder<TRequest, TStartup> As(UserDto user)
        {
            this.user = user;
            return this;
        }

        public async Task<TRequest> ExecuteAndRead()
        {
            HttpResponseMessage response = await Execute();
            return await response.ReadAsAsync<TRequest>();
        }

        public async Task<HttpResponseMessage> Execute()
        {
            using HttpClient client = factory.CreateClient();

            client.SetIntent(SubmitIntent.Execute);
            if (user != null)
            {
                if (user.Id != null)
                {
                    client.SetUserId(user.Id);
                }

                if (user.Email != null)
                {
                    client.SetUsername(user.Email);
                }
            }

            if (string.IsNullOrEmpty(url))
            {
                throw new NullReferenceException(nameof(url));
            }

            HttpResponseMessage response = await client.PostAsJsonAsync(url, request);
            return response;
        }
    }

    public enum SubmitIntent
    {
        Execute = 1,
        Validate = 2
    }

    public static class ClientExtensions
    {
        public static void SetIntent(this HttpClient client, SubmitIntent intent)
        {
            if (intent == SubmitIntent.Execute)
            {
                client.DefaultRequestHeaders.Add("x-submit-intent", "execute");
            }
            else if (intent == SubmitIntent.Validate)
            {
                client.DefaultRequestHeaders.Add("x-submit-intent", "validate");
            }
        }

        public static void SetUser(this HttpClient client, UserDto user)
        {
            if (user.Id == null)
            {
                throw new ArgumentException("User.Id is null");
            }

            client.SetUserId(user.Id);
            if (user.Email != null)
            {
                client.SetUsername(user.Email);
            }
        }

        public static void SetUsername(this HttpClient client, string userName)
        {
            client.DefaultRequestHeaders.Add("x-username", userName);
        }

        public static void SetUserId(this HttpClient client, string userId)
        {
            client.DefaultRequestHeaders.Add("x-userid", userId);
        }
    }
}