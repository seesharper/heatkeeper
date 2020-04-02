using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using HeatKeeper.Server.Host.Locations;
using HeatKeeper.Server.Host.Users;
using HeatKeeper.Server.Locations;
using HeatKeeper.Server.Zones;
using Xunit;

namespace HeatKeeper.Server.WebApi.Tests
{
    public class LocationsTests : TestBase
    {
        [Fact]
        public async Task ShouldCreateLocation()
        {
            var client = Factory.CreateClient();
            var token = await client.AuthenticateAsAdminUser();

            var response = await client.CreateLocation(TestData.Locations.Home, token);

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var content = await response.ContentAs<CreateLocationResponse>();
            content.Id.Should().Be(1);

            var createdLocation = (await client.GetLocations(token)).Single();
            createdLocation.Id.Should().Be(1);
            createdLocation.Name.Should().Be(TestData.Locations.Home.Name);
            createdLocation.Description.Should().Be(TestData.Locations.Home.Description);

            response.Headers.Should().Contain(header => header.Key == "Location");
        }

        [Fact]
        public async Task ShouldUpdateLocation()
        {
            var client = Factory.CreateClient();
            var token = await client.AuthenticateAsAdminUser();

            var response = await client.CreateLocation(TestData.Locations.Home, token);

            var content = await response.ContentAs<CreateLocationResponse>();

            var updateLocationCommand = new UpdateLocationCommand()
            {
                LocationId = content.Id,
                Name = TestData.Locations.Cabin.Name,
                Description = TestData.Locations.Cabin.Description
            };

            await client.PatchLocation(updateLocationCommand, token);

            var updatedLocation = (await client.GetLocations(token)).Single();

            updatedLocation.Name.Should().Be(TestData.Locations.Cabin.Name);
            updatedLocation.Description.Should().Be(TestData.Locations.Cabin.Description);
            updatedLocation.Id.Should().Be(content.Id);
        }


        [Fact]
        public async Task ShouldCreateLocationOnlyForAdminUser()
        {
            var client = Factory.CreateClient();
            var token = await client.CreateAndAuthenticateStandardUser();
            var createLocationResponse = await client.CreateLocation(TestData.Locations.Home, token);
            createLocationResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task ShouldCreateAndGetLocations()
        {
            var client = Factory.CreateClient();
            var token = await client.AuthenticateAsAdminUser();

            await client.CreateLocation(TestData.Locations.Home, token);
            await client.CreateLocation(TestData.Locations.Cabin, token);

            var locations = await client.GetLocations(token);

            locations.Length.Should().Be(2);
        }

        [Fact]
        public async Task ShouldHandleDuplicateLocation()
        {
            var client = Factory.CreateClient();
            var token = await client.AuthenticateAsAdminUser();

            await client.CreateLocation(TestData.Locations.Home, token);
            var response = await client.CreateLocation(TestData.Locations.Home, token);

            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }



        [Fact]
        public async Task ShouldCreateAndGetZones()
        {
            var client = Factory.CreateClient();
            var token = await client.AuthenticateAsAdminUser();
            var locationMessageResponse = await client.CreateLocation(TestData.Locations.Home, token);
            var locationId = (await locationMessageResponse.ContentAs<CreateLocationResponse>()).Id;
            await client.CreateZone(locationId, TestData.Zones.LivingRoom);
            await client.CreateZone(locationId, TestData.Zones.Kitchen);

            var zones = await (await client.GetZones(locationId)).ContentAs<Zone[]>();
            zones.Length.Should().Be(2);
        }

        [Fact]
        public async Task ShouldBeAbleToCreateSameZoneInDifferentLocations()
        {
            var client = Factory.CreateClient();
            var token = await client.AuthenticateAsAdminUser();


            var locationMessageResponse = await client.CreateLocation(TestData.Locations.Home, token);
            var homeLocationId = (await locationMessageResponse.ContentAs<CreateLocationResponse>()).Id;
            locationMessageResponse = await client.CreateLocation(TestData.Locations.Cabin, token);
            var cabinLocationID = (await locationMessageResponse.ContentAs<CreateLocationResponse>()).Id;

            await client.CreateZone(homeLocationId, TestData.Zones.LivingRoom);
            await client.CreateZone(cabinLocationID, TestData.Zones.LivingRoom);

            var zones = await (await client.GetZones(homeLocationId)).ContentAs<Zone[]>();
            zones.Length.Should().Be(1);

            zones = await (await client.GetZones(cabinLocationID)).ContentAs<Zone[]>();
            zones.Length.Should().Be(1);
        }

        [Fact]
        public async Task ShouldHandleCreatingDuplicateZones()
        {
            var client = Factory.CreateClient();
            var token = await client.AuthenticateAsAdminUser();

            var locationRequestMessage = await client.CreateLocation(TestData.Locations.Home, token);
            var locationId = (await locationRequestMessage.ContentAs<CreateLocationResponse>()).Id;

            await client.CreateZone(locationId, TestData.Zones.LivingRoom);

            var zoneRequestMessage = await client.CreateZone(locationId, TestData.Zones.LivingRoom);

            zoneRequestMessage.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task ShouldSetZoneAsDefaultOutsideZone()
        {
            var client = Factory.CreateClient();
            var token = await client.AuthenticateAsAdminUser();

            var locationRequestMessage = await client.CreateLocation(TestData.Locations.Home, token);
            var locationId = (await locationRequestMessage.ContentAs<CreateLocationResponse>()).Id;

            await client.CreateZone(locationId, TestData.Zones.Outside);
            await client.CreateZone(locationId, TestData.Zones.LivingRoom);

            var zones = await (await client.GetZones(locationId)).ContentAs<Zone[]>();

            var outsideZone = zones.Single(z => z.Name == TestData.Zones.Outside.Name);

            var zoneDetailsResponse = await client.GetZoneDetails(token, outsideZone.Id);
            var zoneDetail = await zoneDetailsResponse.ContentAs<ZoneDetails>();

            zoneDetail.IsDefaultOutsideZone.Should().Be(true);
        }

        [Fact]
        public async Task ShouldAddUserToLocation()
        {
            var client = Factory.CreateClient();

            var token = await client.AuthenticateAsAdminUser();

            var createLocationResponse = await client.CreateLocation(TestData.Locations.Home, token);
            var location = await createLocationResponse.ContentAs<CreateLocationResponse>();

            var createUserResponse = await client.RegisterUser(TestData.Users.StandardUser, token);
            var user = await createUserResponse.ContentAs<RegisterUserResponse>();

            var response = await client.AddUserToLocation(location.Id, new AddUserToLocationCommand(user.Id), token);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task ShouldHandleAddingDuplicateUsers()
        {
            var client = Factory.CreateClient();
            var token = await client.AuthenticateAsAdminUser();

            var createLocationResponseMessage = await client.CreateLocation(TestData.Locations.Home, token);
            var locationId = (await createLocationResponseMessage.ContentAs<CreateLocationResponse>()).Id;

            var registerUserResponseMessage = await client.RegisterUser(TestData.Users.StandardUser, token);
            var userId = (await registerUserResponseMessage.ContentAs<RegisterUserResponse>()).Id;

            var response = await client.AddUserToLocation(locationId, new AddUserToLocationCommand(userId), token);
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            response = await client.AddUserToLocation(locationId, new AddUserToLocationCommand(userId), token);

            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }


        [Fact]
        public async Task ShouldRemoveUserFromLocation()
        {
            var client = Factory.CreateClient();
            var token = await client.AuthenticateAsAdminUser();
            var createLocationResponse = await client.CreateLocation(TestData.Locations.Home, token);
            var location = await createLocationResponse.ContentAs<CreateLocationResponse>();

            var createUserResponseMessage = await client.RegisterUser(TestData.Users.StandardUser, token);
            var user = await createUserResponseMessage.ContentAs<RegisterUserResponse>();

            var addUserLocationResponseMessage = await client.AddUserToLocation(location.Id, new AddUserToLocationCommand(user.Id), token);
            addUserLocationResponseMessage.EnsureSuccessStatusCode();

            var removeUserFromLocationResponseMessage = await client.RemoveUserFromRequest(location.Id, user.Id);
            removeUserFromLocationResponseMessage.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
    }
}
