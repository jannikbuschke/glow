using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Glow.Tests
{
    public static class HttpContentExtension
    {
        public static async Task<T> ReadAsAsync<T>(this HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                T result = JToken.Parse(content).ToObject<T>();
                return result;
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new System.Exception(response.ReasonPhrase + ": " + content);
            }
        }
    }
}