using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Glow.Users;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json.Linq;

namespace Glow.Tests
{
    public class SendBuilder<TResponse, TStartup> where TStartup : class
    {
        private readonly WebApplicationFactory<TStartup> factory;
        private readonly Func<HttpClient> clientFactory;
        private readonly object request;
        private string url;
        private UserDto user;
        private readonly bool useSystemTextJson;

        public SendBuilder(Func<HttpClient> clientFactory, object request, bool useSystemTextJson = false)
        {
            this.clientFactory = clientFactory;
            this.request = request;
            this.useSystemTextJson = useSystemTextJson;
        }

        public SendBuilder(WebApplicationFactory<TStartup> factory, object request, bool useSystemTextJson = false)
        {
            this.factory = factory;
            this.request = request;
            this.useSystemTextJson = useSystemTextJson;
        }

        public SendBuilder<TResponse, TStartup> To(string url)
        {
            this.url = url;
            return this;
        }

        public SendBuilder<TResponse, TStartup> As(UserDto user)
        {
            this.user = user;
            return this;
        }

        public async Task<TResponse> ExecuteAndRead()
        {
            HttpResponseMessage response = await Execute();
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Response is not successfull: " + content);
            }
            if (useSystemTextJson)
            {
                var options = new JsonSerializerOptions();
                JsonSerializationSettings.ConfigureStjSerializerDefaultsForWeb(options);
                options.PropertyNameCaseInsensitive = true;
                TResponse result00 = System.Text.Json.JsonSerializer.Deserialize<TResponse>(content, options);
                return result00;
            }
            else
            {
                TResponse result = JToken.Parse(content).ToObject<TResponse>();
                return result;
            }
        }

        public async Task<HttpResponseMessage> Execute()
        {
            using HttpClient client = clientFactory != null ? clientFactory.Invoke() : factory.CreateClient();

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

            string GetUrlFromRequestActionAttribute()
            {
                var type = request.GetType();
                var attributes = type.Attributes;
                var customAttributes = type.CustomAttributes;
                var action = customAttributes.FirstOrDefault(v =>
                    v.AttributeType.GetInterface("Glow.Core.Actions.IAction") != null);
                var routePreoprty = action?.NamedArguments.FirstOrDefault(v => v.MemberName == "Route");
                var routeValue = routePreoprty?.TypedValue.Value as string;
                return routeValue;
            }

            var route = string.IsNullOrEmpty(url) ? GetUrlFromRequestActionAttribute() : url;

            if (string.IsNullOrEmpty(route))
            {
                throw new NullReferenceException(nameof(route));
            }

            var options = new JsonSerializerOptions();
            JsonSerializationSettings.ConfigureStjSerializerDefaultsForWeb(options);
            var data = useSystemTextJson ? System.Text.Json.JsonSerializer.Serialize(request, options) : JObject.FromObject(request).ToString();
            HttpResponseMessage response = await client.PostAsync(route, new StringContent(data, Encoding.UTF8, "application/json"));

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
        public static async Task<Result> PostRequest<Action, Result>(this HttpClient client, Action request, SubmitIntent intent, UserDto user, string url)
        {
            client.SetIntent(intent);
            client.SetUser(user);
            var data = JObject.FromObject(request).ToString();
            HttpResponseMessage response = await client.PostAsync(url, new StringContent(data, Encoding.UTF8, "application/json"));
            var result = await response.ReadAsAsync<Result>();
            return result;
        }

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
