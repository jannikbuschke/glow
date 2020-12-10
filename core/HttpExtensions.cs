using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Glow.Core
{
    public static class HttpExtensions
    {
        public static Task WriteJson<T>(this HttpResponse response, T obj, string contentType = null)
        {
            response.ContentType = contentType ?? "application/json";
            var result = JsonSerializer.Serialize(obj);
            return response.WriteAsync(result);
        }
    }
}
