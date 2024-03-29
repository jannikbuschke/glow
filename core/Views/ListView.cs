using System;
using System.Linq;
using System.Threading.Tasks;
using Glow.Core.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Glow.Core.Views
{
    [ApiController]
    public abstract class ListView<T> : ControllerBase
    {
        protected abstract IQueryable<T> Get(string search);

        [PaginateFilter]
        [HttpGet]
        [Obsolete("use /query instead")]
        public IQueryable<T> Query(string search)
        {
            return Get(search);
        }

        [HttpPost]
        [HttpPost("query")]
        public ActionResult<QueryResult<T>> Query(Query query)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Get(query.Search).Apply(query);
        }
    }

    [ApiController]
    public abstract class AsyncListView<T> : ControllerBase
    {
        protected abstract Task<IQueryable<T>> Get(string search);

        [HttpPost]
        public async Task<ActionResult<QueryResult<T>>> Query(Query query)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var q = await Get(query.Search);
            return q.Apply(query);
        }
    }

    [ApiController]
    public abstract class QueryableController<T> : ControllerBase
    {
        protected abstract Task<QueryResult<T>> OnQuery(Query query);

        [HttpPost("query")]
        [HttpPost]
        public Task<QueryResult<T>> Query(Query query)
        {
            return OnQuery(query);
        }
    }
}