using AutoFixture;
using FluentAssertions;
using HeatKeeper.Server.Host.Locations;
using HeatKeeper.Server.Host.Users;
using HeatKeeper.Server.Host.Zones;
using HeatKeeper.Server.WebApi.Tests.Customizations;
using LightInject;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HeatKeeper.Server.WebApi.Tests
{

    public class LocationsTests : TestBase
    {
        public LocationsTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            Fixture.Customizations.Add(new MailAddressCustomization());
            Fixture.Customizations.Add(new PasswordCustomization());
        }

        [Fact]
        public async Task ShouldCreateLocation()
        {
            var client = Factory.CreateClient();
            var request = Fixture.Create<CreateLocationRequest>();

            var response = await client.CreateLocation(request);

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var content = await response.ContentAs<CreateLocationResponse>();
            content.Id.Should().Be(1);
            response.Headers.Should().Contain(header => header.Key == "Location");
        }


        [Fact]
        public async Task ShouldCreateAndGetLocations()
        {
            var client = Factory.CreateClient();

            var firstRequest = Fixture.Create<CreateLocationRequest>();
            var secondRequest = Fixture.Create<CreateLocationRequest>();

            await client.CreateLocation(firstRequest);
            await client.CreateLocation(secondRequest);

            var locations = await client.GetLocations();

            locations.Length.Should().Be(2);
        }

        [Fact]
        public async Task ShouldCreateAndGetZones()
        {
            var client = Factory.CreateClient();
            var locationMessageResponse = await client.CreateLocation(Fixture.Create<CreateLocationRequest>());
            var locationId = (await locationMessageResponse.ContentAs<CreateLocationResponse>()).Id;
            await client.CreateZone(locationId, Fixture.Create<CreateZoneRequest>());
            await client.CreateZone(locationId, Fixture.Create<CreateZoneRequest>());

            var zones = await (await client.GetZones(locationId)).ContentAs<ZoneResponse[]>();
            zones.Length.Should().Be(2);
        }

        [Fact]
        public async Task ShouldBeAbleToCreateSameZoneInDifferentLocations()
        {
            var client = Factory.CreateClient();
            var locationMessageResponse = await client.CreateLocation(Fixture.Create<CreateLocationRequest>());
            var firstLocationId = (await locationMessageResponse.ContentAs<CreateLocationResponse>()).Id;
            locationMessageResponse = await client.CreateLocation(Fixture.Create<CreateLocationRequest>());
            var secondLocationId = (await locationMessageResponse.ContentAs<CreateLocationResponse>()).Id;

            var createZoneRequest = Fixture.Create<CreateZoneRequest>();

            await client.CreateZone(firstLocationId, createZoneRequest);
            await client.CreateZone(secondLocationId, createZoneRequest);

            var zones = await (await client.GetZones(firstLocationId)).ContentAs<ZoneResponse[]>();
            zones.Length.Should().Be(1);

            zones = await (await client.GetZones(secondLocationId)).ContentAs<ZoneResponse[]>();
            zones.Length.Should().Be(1);
        }


        [Fact]
        public async Task ShouldHandleDuplicateLocation()
        {
            var client = Factory.CreateClient();

            var request = Fixture.Create<CreateLocationRequest>();

            var locationRequestMessage = await client.CreateLocation(request);
            locationRequestMessage.EnsureSuccessStatusCode();

            locationRequestMessage = await client.CreateLocation(request);

            locationRequestMessage.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task ShouldHandleCreatingDuplicateZones()
        {
            var client = Factory.CreateClient();

            var createLocationRequest = Fixture.Create<CreateLocationRequest>();

            var locationRequestMessage = await client.CreateLocation(createLocationRequest);
            var locationId = (await locationRequestMessage.ContentAs<CreateLocationResponse>()).Id;

            var createZoneRequest = Fixture.Create<CreateZoneRequest>();

            var zoneRequestMessage = await client.CreateZone(locationId, createZoneRequest);
            zoneRequestMessage.EnsureSuccessStatusCode();

            zoneRequestMessage = await client.CreateZone(locationId, createZoneRequest);

            zoneRequestMessage.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task ShouldAddUserToLocation()
        {
            var client = Factory.CreateClient();

            var createLocationResponse = await client.CreateLocation(Fixture.Create<CreateLocationRequest>());
            var location = await createLocationResponse.ContentAs<CreateLocationResponse>();

            var createUserResponse = await client.CreateUser(Fixture.Create<RegisterUserRequest>());
            var user = await createUserResponse.ContentAs<RegisterUserResponse>();

            var response = await client.AddUserToLocation(new AddUserLocationRequest(user.Id, location.Id));
            response.EnsureSuccessStatusCode();

        }
    }
}
