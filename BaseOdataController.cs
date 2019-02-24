using System.Linq;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;

namespace JannikB.AspNetCore.Utils.Module
{

    public abstract class BaseOdataController<T, K>
        : ODataController where T : IEntity<K>
    {
        private readonly IQueryable<T> q;

        public BaseOdataController(IQueryable<T> q)
        {
            this.q = q;
        }

        [ODataRoute("{key}")]
        [EnableQuery]
        public SingleResult<T> Get([FromODataUri] K key)
        {
            return SingleResult.Create(q.Where(v => key.Equals(v.Id)));
        }

        [ODataRoute]
        [EnableQuery]
        public IQueryable<T> Get()
        {
            return q;
        }
    }
}
