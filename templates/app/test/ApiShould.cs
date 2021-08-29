using System.Net.Http;
using Xunit;

namespace TemplateName.Test
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
