using System;
using System.Net.Http;
using System.Text;
using HeatKeeper.Server.Host;
using HeatKeeper.Server.WebApi.Zones;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace HeatKeeper.Server.WebApi.Tests
{
    public class ZonesTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public ZonesTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public void TestShouldCreateZone()
        {
            var client = _factory.CreateClient();
            var response = client.PostAsync("api/zones", new JsonContent(new CreateZoneRequest("1", "TEST")));
        }
    }

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
