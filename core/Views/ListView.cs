using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Glow.Core.Views
{
    [ApiController]
    public abstract class ListView<T> : ControllerBase
    {
        protected abstract IQueryable<T> Get();

        [HttpGet]
        public List<T> Get(int? skip = 0, int? take = 10)
        {
            return Get().Skip(skip.Value).Take(take.Value).ToList();
        }
    }
}
