using System.Threading.Tasks;
using Glow.Core.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Glow.Core.Views
{
    [ApiController]
    public abstract class DetailView<T, Key> : ControllerBase
    {
        protected abstract Task<T> Get(Key id);

        [PaginateFilter]
        [HttpGet("{id}")]
        public async Task<ActionResult<T>> GetSingle(Key id)
        {
            var result = await Get(id);
            if (result == null)
            {
                return NotFound();
            }
            return result;
        }
    }
}
