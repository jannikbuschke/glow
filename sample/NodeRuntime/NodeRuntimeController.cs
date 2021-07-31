using System.Threading.Tasks;
using Jering.Javascript.NodeJS;
using Microsoft.AspNetCore.Mvc;

namespace Glow.Sample.NodeRuntime
{
    [Route("api/node")]
    public class NodeRuntimeController : ControllerBase
    {
        private readonly INodeJSService node;

        public NodeRuntimeController(INodeJSService node)
        {
            this.node = node;
        }

        [HttpGet]
        public async Task<string> Get()
        {
            string result1 = await node.InvokeFromFileAsync<string>("./interop.js", args: new[] {"testString"})
                .ConfigureAwait(false);
            return result1;
        }
    }
}
