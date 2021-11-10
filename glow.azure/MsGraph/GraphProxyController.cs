using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Glow.Core.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Newtonsoft.Json.Linq;

namespace Glow.MsGraph
{
    [Authorize]
    [Route("api/graph-search")]
    public class TeamsController : ControllerBase
    {
        private readonly IGraphTokenService graphTokenService;
        private readonly IMemoryCache memoryCache;

        public TeamsController(IGraphTokenService graphTokenService, IMemoryCache memoryCache)
        {
            this.graphTokenService = graphTokenService;
            this.memoryCache = memoryCache;
        }

        public static string GetCacheKey<T>()
        {
            return typeof(T).FullName;
        }

        // ReSharper disable once ArrangeAccessorOwnerBody
        private static string TeamsKey => GetCacheKey<Team>();

        // ReSharper disable once ArrangeAccessorOwnerBody
        private static string ChannelKey => GetCacheKey<Channel>();

        [HttpGet("me/joined-teams")]
        public async Task<List<Team>> MyJoinedTeams()
        {
            GraphServiceClient client = await graphTokenService.GetClientForUser(new[] { "profile" });
            IUserJoinedTeamsCollectionPage page = await client.Me.JoinedTeams.Request().GetAsync();
            var data = page.ToList();

            // AddToCache(data, TeamsKey);

            return data;
        }

        [HttpGet("team/{teamId}/channels")]
        public async Task<List<Channel>> Channels(string teamId)
        {
            GraphServiceClient client = await graphTokenService.GetClientForUser(new[] { "profile" });
            ITeamChannelsCollectionPage page = await client
                .Teams[teamId]
                .Channels
                .Request()
                .GetAsync();
            var data = page.ToList();

            // AddToCache(data, ChannelKey);

            return data;
        }

        private void AddToCache<T>(List<T> data, string cacheKey) where T : Microsoft.Graph.Entity
        {
            var cached = new Dictionary<string, T>();

            if (!memoryCache.TryGetValue(TeamsKey, out cached))
            {
                cached = data.ToDictionary(v => v.Id, v => v);
            }
            else
            {
                foreach (T v in data)
                {
                    cached[v.Id] = v;
                }
            }

            MemoryCacheEntryOptions options =
                new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(10));
            memoryCache.Set(cacheKey, cached, options);
        }
    }

    [ApiVersion("1.0")]
    [Authorize]
    [Route("api/graph")]
    public class GraphProxyController : ControllerBase
    {
        private readonly IHttpClientFactory factory;
        private readonly IGraphTokenService graphTokenService;
        private readonly ILogger<GraphProxyController> logger;
        private readonly IMemoryCache memoryCache;

        public GraphProxyController(
            IHttpClientFactory factory,
            IGraphTokenService graphTokenService,
            ILogger<GraphProxyController> logger,
            IMemoryCache memoryCache
        )
        {
            this.factory = factory;
            this.graphTokenService = graphTokenService;
            this.logger = logger;
            this.memoryCache = memoryCache;
        }

        [HttpGet]
        public async Task<ActionResult<object>> Get(string path)
        {
            try
            {
                HttpClient client = factory.CreateClient();
                client.BaseAddress = new Uri("https://graph.microsoft.com");
                var accessToken = await graphTokenService.AccessTokenForCurrentUser(new string[] { "openid" });
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var uri = "https://graph.microsoft.com/" + path;
                logger.LogInformation("GET {uri}", uri);
                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return Ok(JObject.Parse(content).ToObject<dynamic>());
                }
                else
                {
                    var msg = path + await response.Content.ReadAsStringAsync();
                    return StatusCode((int) response.StatusCode, msg);
                }
            }
            catch (Exception e)
            {
                return Problem(detail: e.Message, statusCode: (int) HttpStatusCode.InternalServerError,
                    title: "Server Error");
            }
        }
    }
}