using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace HeatKeeper.Server.WebApi.Tests
{
    public class JsonContent : StringContent
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        public JsonContent(object value)
            : base(JsonSerializer.Serialize(value, value.GetType(), Options), Encoding.UTF8, "application/json")
        {
        }

        public JsonContent(object value, string mediaType)
            : base(JsonSerializer.Serialize(value, value.GetType(), Options), Encoding.UTF8, mediaType)
        {
        }
    }
}
