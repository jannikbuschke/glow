using System;
using System.Net.Http;
using System.Threading.Tasks;
using Bogus;
using Glow.Clocks;
using Glow.Core.Queries;
using Glow.Glue.AspNetCore.Tests;
using Glow.Tests;
using Glow.Users;
using MediatR;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Glow.Core.Tests
{
    public abstract class BaseIntegrationTest<Startup> : IDisposable where Startup : class
    {
        protected readonly HttpClient client;
        protected Faker Faker { get; set; } = new Faker();
        public BaseIntegrationTest(WebApplicationFactory<Startup> factory)
        {
            Factory = factory;
            client = Factory.CreateClient();
        }

        private FakeClock clock;
        protected FakeClock Clock
        {
            get
            {
                if (clock == null)
                {
                    clock = GetRequiredService<FakeClock>();
                }
                return clock;
            }
        }

        public WebApplicationFactory<Startup> Factory { get; }

        public void Dispose()
        {
            // client.Dispose();
            // Factory.Dispose();
        }

        protected T GetRequiredService<T>()
        {
            return Factory.Server.Host.Services.GetRequiredService<T>();
        }

        protected Task<T> SendRequest<T>(IRequest<T> request)
        {
            IMediator m = GetRequiredService<IMediator>();
            return m.Send(request);
        }

        public SendBuilder<TResponse, Startup> Send<TResponse>(IRequest<TResponse> request)
        {
            return new(Factory, request);
        }

        public async Task<byte[]> Download(string url, UserDto asUser, IRequest<byte[]> request)
        {
            var client = Factory.CreateClient();
            client.SetUser(asUser);
            client.SetIntent(SubmitIntent.Execute);
            var result = await client.PostAsJsonAsync(url, request);
            if (!result.IsSuccessStatusCode)
            {
                var error = await result.Content.ReadAsStringAsync();
            }
            result.EnsureSuccessStatusCode();
            var data = await result.Content.ReadAsByteArrayAsync();
            return data;
        }

        protected QueryBuilder<Startup> Query(string url)
        {
            return new QueryBuilder<Startup>(Factory, url);
        }

        protected QueryBuilder<Startup> QueryList(string url, Query query = null)
        {
            return new QueryBuilder<Startup>(Factory, url, query ?? new Query());
        }

        protected GetBuilder<Startup> Get()
        {
            return new GetBuilder<Startup>(Factory);
        }
    }
}
