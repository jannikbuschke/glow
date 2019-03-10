using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;

namespace JannikB.AspNetCore.Utils.Module
{

    public abstract class BaseOdataController<T, K> : ODataController where K : IEquatable<K>
    {
        private readonly IQueryable<T> q;
        private readonly Func<string, IQueryable<T>> queryMany;
        private readonly Func<K, IQueryable<T>> querySingle;

        public BaseOdataController(IQueryable<T> q, Func<K, IQueryable<T>> querySingle)
        {
            this.q = q;
            this.querySingle = querySingle;
        }

        public BaseOdataController(Func<string, IQueryable<T>> queryMany, Func<K, IQueryable<T>> querySingle)
        {
            this.queryMany = queryMany;
            this.querySingle = querySingle;
        }

        [ODataRoute("{key}")]
        [EnableQuery]
        public SingleResult<T> Get([FromODataUri] K key)
        {
            return SingleResult.Create(querySingle(key));
        }

        [ODataRoute]
        [EnableQuery]
        public IQueryable<T> Get(string search = "")
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
