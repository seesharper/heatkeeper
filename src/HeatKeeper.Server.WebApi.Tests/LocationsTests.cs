using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
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

            var locationId = await client.PostLocation(TestData.Locations.Home, token);

            var createdLocation = (await client.GetLocations(token)).Single();

            createdLocation.Id.Should().Be(locationId);
            createdLocation.Name.Should().Be(TestData.Locations.Home.Name);
            createdLocation.Description.Should().Be(TestData.Locations.Home.Description);

            //response.Headers.Should().Contain(header => header.Key == "Location");
        }

        [Fact]
        public async Task ShouldUpdateLocation()
        {
            var client = Factory.CreateClient();
            var token = await client.AuthenticateAsAdminUser();

            var locationId = await client.PostLocation(TestData.Locations.Home, token);

            var updateLocationCommand = new UpdateLocationCommand()
            {
                Id = locationId,
                Name = TestData.Locations.Cabin.Name,
                Description = TestData.Locations.Cabin.Description
            };

            await client.PatchLocation(updateLocationCommand, token);

            var updatedLocation = (await client.GetLocations(token)).Single();

            updatedLocation.Name.Should().Be(TestData.Locations.Cabin.Name);
            updatedLocation.Description.Should().Be(TestData.Locations.Cabin.Description);
            updatedLocation.Id.Should().Be(locationId);
        }

        [Fact]
        public async Task ShouldCreateLocationOnlyForAdminUser()
        {
            var client = Factory.CreateClient();
            var token = await client.CreateAndAuthenticateStandardUser();

            await client.PostLocation(TestData.Locations.Home, token, problem: details => details.ShouldHaveUnauthorizedStatus());
        }

        [Fact]
        public async Task ShouldCreateAndGetLocations()
        {
            var client = Factory.CreateClient();
            var token = await client.AuthenticateAsAdminUser();

            await client.PostLocation(TestData.Locations.Home, token);
            await client.PostLocation(TestData.Locations.Cabin, token);

            var locations = await client.GetLocations(token);

            locations.Length.Should().Be(2);
        }

        [Fact]
        public async Task ShouldHandleCreatingDuplicateLocations()
        {
            var client = Factory.CreateClient();
            var token = await client.AuthenticateAsAdminUser();

            await client.PostLocation(TestData.Locations.Home, token);
            await client.PostLocation(TestData.Locations.Home, token, problem: details => details.ShouldHaveConflictStatus());
        }

        [Fact]
        public async Task ShouldHandleDuplicateLocationsOnUpdate()
        {
            var client = Factory.CreateClient();
            var token = await client.AuthenticateAsAdminUser();

            await client.PostLocation(TestData.Locations.Home, token);
            var cabinLocationId = await client.PostLocation(TestData.Locations.Cabin, token);

            var updateLocationCommand = new UpdateLocationCommand()
            {
                Id = cabinLocationId,
                Name = TestData.Locations.Home.Name,
                Description = TestData.Locations.Home.Description
            };

            await client.PatchLocation(updateLocationCommand, token, problem: details => details.ShouldHaveConflictStatus());
        }

        [Fact]
        public async Task ShouldCreateAndGetZones()
        {
            var client = Factory.CreateClient();
            var token = await client.AuthenticateAsAdminUser();
            var locationId = await client.PostLocation(TestData.Locations.Home, token);
            await client.PostZone(locationId, TestData.Zones.LivingRoom, token);
            await client.PostZone(locationId, TestData.Zones.Kitchen, token);

            var zones = await client.GetZones(locationId);
            zones.Length.Should().Be(2);
        }

        [Fact]
        public async Task ShouldBeAbleToCreateSameZoneInDifferentLocations()
        {
            var client = Factory.CreateClient();
            var token = await client.AuthenticateAsAdminUser();

            var homeLocationId = await client.PostLocation(TestData.Locations.Home, token);
            var cabinLocationID = await client.PostLocation(TestData.Locations.Cabin, token);

            await client.PostZone(homeLocationId, TestData.Zones.LivingRoom, token);
            await client.PostZone(cabinLocationID, TestData.Zones.LivingRoom, token);

            var zones = await client.GetZones(homeLocationId);
            zones.Length.Should().Be(1);

            zones = await client.GetZones(cabinLocationID);
            zones.Length.Should().Be(1);
        }

        [Fact]
        public async Task ShouldHandleCreatingDuplicateZones()
        {
            var client = Factory.CreateClient();
            var token = await client.AuthenticateAsAdminUser();

            var locationId = await client.PostLocation(TestData.Locations.Home, token);

            await client.PostZone(locationId, TestData.Zones.LivingRoom, token);
            await client.PostZone(locationId, TestData.Zones.LivingRoom, token, problem: details => details.ShouldHaveConflictStatus());
        }

        [Fact]
        public async Task ShouldSetZoneAsDefaultOutsideZone()
        {
            var client = Factory.CreateClient();
            var token = await client.AuthenticateAsAdminUser();

            var locationId = await client.PostLocation(TestData.Locations.Home, token);

            await client.PostZone(locationId, TestData.Zones.Outside, token);
            await client.PostZone(locationId, TestData.Zones.LivingRoom, token);

            var zones = await client.GetZones(locationId);

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

            var locationId = await client.PostLocation(TestData.Locations.Home, token);

            var userId = await client.PostUser(TestData.Users.StandardUser, token);

            var response = await client.AddUserToLocation(locationId, new AddUserToLocationCommand(userId), token);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task ShouldHandleAddingDuplicateUsers()
        {
            var client = Factory.CreateClient();
            var token = await client.AuthenticateAsAdminUser();

            var locationId = await client.PostLocation(TestData.Locations.Home, token);

            var userId = await client.PostUser(TestData.Users.StandardUser, token);

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
            var locationId = await client.PostLocation(TestData.Locations.Home, token);
            var userId = await client.PostUser(TestData.Users.StandardUser, token);


            var addUserLocationResponseMessage = await client.AddUserToLocation(locationId, new AddUserToLocationCommand(userId), token);
            addUserLocationResponseMessage.EnsureSuccessStatusCode();

            var removeUserFromLocationResponseMessage = await client.RemoveUserFromRequest(locationId, userId);
            removeUserFromLocationResponseMessage.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
    }
}
