using System.Net.Http;
using Glow.Sample;
using Xunit;

namespace Glow.Test
{
    public class ApiShould : BaseIntegrationTestClass
    {
        public ApiShould(CustomWebApplicationFactory<Startup> factory) : base(factory) { }

        [Fact]
        public async void Not_Throw()
        {
            HttpResponseMessage response = await client.GetAsync("/hello");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.Equal("hello world", content);
        }
    }
}
