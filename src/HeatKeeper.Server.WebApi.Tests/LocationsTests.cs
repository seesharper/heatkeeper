using System;
using System.Threading.Tasks;
using HeatKeeper.Abstractions.CQRS;
using HeatKeeper.Server.Host;
using HeatKeeper.Server.Host.Locations;
using HeatKeeper.Abstractions.Logging;
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
using System.Net.Http;
using AutoFixture;

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

        public Fixture Fixture => new Fixture();

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
            var response = await client.CreateLocation(Fixture.Create<CreateLocationRequest>());
            response.EnsureSuccessStatusCode();
            response.ContentAs<CreateLocationResponse>().Id.Should().Be(1);
        }

        [Fact]
        public async Task ShouldGetLocations()
        {
            var client = Factory.CreateClient();
            var firstRequest = Fixture.Create<CreateLocationRequest>();
            var secondRequest = Fixture.Create<CreateLocationRequest>();
            await client.CreateLocation(firstRequest);
            await client.CreateLocation(secondRequest);

            var locations = await client.GetAsync<GetLocationsResponse[]>("api/locations");

            locations.Length.Should().Be(2);
        }

        [Fact]
        public async Task ShouldAddUser()
        {
            var client = Factory.CreateClient();
            var token = await client.AuthenticateAsAdminUser();
            var createLocationRequest = new HttpRequestBuilder()
                .AddMethod(HttpMethod.Post)
                .AddBearerToken(token)
                .AddRequestUri("api/locations")
                .AddContent(new JsonContent(TestRequests.CreateTestLocationRequest))
                .Build();

            await client.SendAsync(createLocationRequest);
        }
    }
}