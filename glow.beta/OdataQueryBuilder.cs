using System;
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
}