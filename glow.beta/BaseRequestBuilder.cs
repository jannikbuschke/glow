using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Glow.Core.Queries;
using Glow.Tests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Glow.Glue.AspNetCore.Tests
{
    public abstract class BaseRequestBuilder<T> where T : class
    {
        private readonly WebApplicationFactory<T> factory;
        protected string Url { get; set; }
        protected string UserId { get; set; }

        public BaseRequestBuilder(WebApplicationFactory<T> factory)
        {
            this.factory = factory;
        }

        protected async Task<QueryResult<R>> PostQueryAndRead<R>(Query query)
        {
            using HttpClient client = factory.CreateClient();

            client.SetIntent(SubmitIntent.Execute);

            client.DefaultRequestHeaders.Add("x-submit-intent", "execute");
            if (!string.IsNullOrWhiteSpace(UserId))
            {
                client.SetUserId(UserId);
            }

            var response = await client.PostAsJsonAsync(Url, query);
            var result = await HandleResponse<QueryResult<R>>(response);
            return result;
        }

        protected async Task<R> Read<R>()
        {
            HttpResponseMessage response = await ExecuteRaw();
            return await HandleResponse<R>(response);
        }

        protected async Task<HttpResponseMessage> ExecuteRaw()
        {
            using HttpClient client = factory.CreateClient();

            client.DefaultRequestHeaders.Add("x-submit-intent", "execute");
            if (!string.IsNullOrWhiteSpace(UserId))
            {
                client.DefaultRequestHeaders.Add("x-userid", UserId);
            }

            HttpResponseMessage response = await client.GetAsync(Url);
            return response;
        }

        private async Task<R> HandleResponse<R>(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                // R responsePayload = await response.Content.ReadAsAsync<R>();
                R responsePayload = await response.Content.ReadFromJsonAsync<R>();
                return responsePayload;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new BadRequestException(content);
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new Exception("Http call was unsuccessfull: " + response.StatusCode + " " +
                                    response.ReasonPhrase + " " + content);
            }
        }
    }
}