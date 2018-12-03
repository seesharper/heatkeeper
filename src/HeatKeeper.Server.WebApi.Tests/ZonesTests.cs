using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HeatKeeper.Server.CQRS;
using HeatKeeper.Server.Host;
using HeatKeeper.Server.WebApi.Tests.Transactions;
using HeatKeeper.Server.WebApi.Zones;
using LightInject;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using Xunit;

namespace HeatKeeper.Server.WebApi.Tests
{
    public class ZonesTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public ZonesTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory.WithWebHostBuilder(builder => {
                builder.ConfigureTestContainer<IServiceContainer>(c => {
                    c.Decorate(typeof(ICommandHandler<>), typeof(RollbackCommandHandler<>));
                });
            });            
        }

        [Fact]
        public async Task TestShouldCreateZone()
        {
            var client = _factory.CreateClient();
            var response = await client.PostAsync("api/zones", new JsonContent(new CreateZoneRequest("1", "TEST")));            
            response.EnsureSuccessStatusCode();                        
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
