using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc.Testing;

namespace JannikB.Glue.AspNetCore.Tests
{
    public class UserDto
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string DisplayName { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
    }

    public static class HttpContentExtension
    {
        public static async Task<T> ReadAsAsync<T>(this HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                T responsePayload = await response.Content.ReadAsAsync<T>();
                return responsePayload;
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new System.Exception(response.ReasonPhrase + ": " + content);
            }
        }
    }

    public class SendCommandBuilder<T, R, Startup> where T : IRequest<R> where Startup : class
    {
        private readonly WebApplicationFactory<Startup> factory;
        private readonly T request;
        private string url;
        private string userId;

        public SendCommandBuilder(WebApplicationFactory<Startup> factory, T request)
        {
            this.factory = factory;
            this.request = request;
        }

        public SendCommandBuilder<T, R, Startup> To(string url)
        {
            this.url = url;
            return this;
        }

        public SendCommandBuilder<T, R, Startup> As(string userId)
        {
            this.userId = userId;
            return this;
        }

        public SendCommandBuilder<T, R, Startup> As(UserDto user)
        {
            userId = user.UserId;
            return this;
        }

        public async Task<R> ExecuteAndRead()
        {
            HttpResponseMessage response = await Execute();
            return await response.ReadAsAsync<R>();
        }

        public async Task<HttpResponseMessage> Execute()
        {
            using HttpClient client = factory.CreateClient();

            client.DefaultRequestHeaders.Add("x-submit-intent", "execute");
            if (!string.IsNullOrWhiteSpace(userId))
            {
                client.DefaultRequestHeaders.Add("x-userid", userId);
            }
            HttpResponseMessage response = await client.PostAsJsonAsync(url, request);
            return response;
        }
    }
}
