using System;
using System.Net.Http;
using System.Threading.Tasks;
using Glow.Core.Queries;
using Glow.Users;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Glow.Glue.AspNetCore.Tests
{
    [Obsolete("Use QueryBuilder<T> instead")]
    public class OdataQueryBuilder<T> : QueryBuilder<T> where T : class
    {
        public OdataQueryBuilder(WebApplicationFactory<T> factory, string url) : base(factory, url)
        {
        }
    }

    public class QueryBuilder<T> : BaseRequestBuilder<T> where T : class
    {
        private Query query;

        public QueryBuilder(WebApplicationFactory<T> factory, string url) : base(factory)
        {
            Url = url;
        }

        public QueryBuilder(WebApplicationFactory<T> factory, string url, Query query) : base(factory)
        {
            this.query = query;
            Url = url;
        }

        public QueryBuilder<T> As(string userId)
        {
            UserId = userId;
            return this;
        }

        public QueryBuilder<T> As(UserDto user)
        {
            UserId = user.Id;
            return this;
        }

        public Task<QueryResult<R>> List<R>()
        {
            return this.PostQueryAndRead<R>(query);
        }

        public new Task<R> Read<R>()
        {
            return base.Read<R>();
        }

        public new Task<HttpResponseMessage> ExecuteRaw()
        {
            return base.ExecuteRaw();
        }
    }
}