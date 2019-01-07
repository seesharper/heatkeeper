using System;
using System.Threading.Tasks;
using HeatKeeper.Server.CQRS;
using HeatKeeper.Server.Host;
using HeatKeeper.Server.Host.Locations;
using HeatKeeper.Server.Logging;
using HeatKeeper.Server.WebApi.Tests.Transactions;
using LightInject;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;
using FluentAssertions;
using System.Linq;
using System.Data;

namespace HeatKeeper.Server.WebApi.Tests
{

    public class TestBase : IDisposable
    {
        private IServiceContainer _container;

        public TestBase(ITestOutputHelper testOutputHelper)
        {
            Factory = new WebApplicationFactory<Startup>()
            .WithWebHostBuilder(builder => {
                builder.ConfigureTestContainer<IServiceContainer>(c => {c.EnableRollback(); _container = c;}).ConfigureLogging(loggingBuilder => loggingBuilder.AddProvider(new TestLoggerProvider()));
            });
            testOutputHelper.Capture();
        }



        public WebApplicationFactory<Startup> Factory { get; }

        public void Dispose()
        {
            _container.Dispose();
        }
    }


    public class LocationsTests : TestBase
    {
        public LocationsTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public async Task ShouldCreateLocation()
        {
            var client = Factory.CreateClient();
            var response = await client.PostAsync("api/locations", new JsonContent(new CreateLocationRequest("Home", "This is my home in Norway")));
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task ShouldGetLocations()
        {
            var client = Factory.CreateClient();

            await client.PostAsync("api/locations", new JsonContent(new CreateLocationRequest("Home", "This is my home in Norway")));
            await client.PostAsync("api/locations", new JsonContent(new CreateLocationRequest("Cabin", "This is my cabin in Sweden")));
            var locations = await client.GetAsync<GetLocationsResponse[]>("api/locations");
            locations.Length.Should().Be(2);
        }
    }
}