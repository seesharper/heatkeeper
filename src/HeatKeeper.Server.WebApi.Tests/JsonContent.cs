using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace HeatKeeper.Server.WebApi.Tests
{
    public class JsonContent : StringContent
    {
        public JsonContent(object value)
            : base (JsonConvert.SerializeObject(value), Encoding.UTF8,
			"application/json")
        {
        }

        public JsonContent(object value, string mediaType)
            : base(JsonConvert.SerializeObject(value), Encoding.UTF8, mediaType)
        {
        }
    }
}
