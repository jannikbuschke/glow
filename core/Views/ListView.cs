using System.Linq;
using Glow.Core.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Glow.Core.Views
{
    [ApiController]
    public abstract class ListView<T> : ControllerBase
    {
        protected abstract IQueryable<T> Get();

        [PaginateFilter]
        [HttpGet]
        public IQueryable<T> Query()
        {
            return Get();
        }
    }
}
