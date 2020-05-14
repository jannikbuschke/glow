using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;

namespace JannikB.AspNetCore.Utils.Module
{
    public abstract class AsyncBaseOdataController<T, K> : ODataController where K : IEquatable<K>
    {
        private readonly Task<IQueryable<T>> q;
        private readonly Func<string, Task<IQueryable<T>>> queryMany;
        private readonly Func<K, Task<IQueryable<T>>> querySingle;

        public AsyncBaseOdataController(Task<IQueryable<T>> q, Func<K, Task<IQueryable<T>>> querySingle)
        {
            this.q = q;
            this.querySingle = querySingle;
        }

        public AsyncBaseOdataController(Func<string, Task<IQueryable<T>>> queryMany, Func<K, Task<IQueryable<T>>> querySingle)
        {
            this.queryMany = queryMany;
            this.querySingle = querySingle;
        }

        [ODataRoute("{key}")]
        [EnableQuery(MaxExpansionDepth = 5)]
        public async Task<SingleResult<T>> Get([FromODataUri] K key)
        {
            return SingleResult.Create(await querySingle(key));
        }

        [ODataRoute]
        [EnableQuery(MaxExpansionDepth = 3)]
        public Task<IQueryable<T>> Get(string search = "")
        {
            if (q != null)
            {
                return q;

            }
            else
            {
                return queryMany(search);
            }
        }
    }
}
