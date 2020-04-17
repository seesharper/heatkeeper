using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using HeatKeeper.Server.Locations;
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

            var locationId = await client.CreateLocation(TestData.Locations.Home, token);

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

            var locationId = await client.CreateLocation(TestData.Locations.Home, token);

            var updateLocationCommand = new UpdateLocationCommand()
            {
                Id = locationId,
                Name = TestData.Locations.Cabin.Name,
                Description = TestData.Locations.Cabin.Description
            };

            await client.UpdateLocation(updateLocationCommand, token);

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

            await client.CreateLocation(TestData.Locations.Home, token, problem: details => details.ShouldHaveUnauthorizedStatus());
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
        public async Task ShouldHandleCreatingDuplicateLocations()
        {
            var client = Factory.CreateClient();
            var token = await client.AuthenticateAsAdminUser();

            await client.CreateLocation(TestData.Locations.Home, token);
            await client.CreateLocation(TestData.Locations.Home, token, problem: details => details.ShouldHaveConflictStatus());
        }

        [Fact]
        public async Task ShouldHandleDuplicateLocationsOnUpdate()
        {
            var client = Factory.CreateClient();
            var token = await client.AuthenticateAsAdminUser();

            await client.CreateLocation(TestData.Locations.Home, token);
            var cabinLocationId = await client.CreateLocation(TestData.Locations.Cabin, token);

            var updateLocationCommand = new UpdateLocationCommand()
            {
                Id = cabinLocationId,
                Name = TestData.Locations.Home.Name,
                Description = TestData.Locations.Home.Description
            };

            await client.UpdateLocation(updateLocationCommand, token, problem: details => details.ShouldHaveConflictStatus());
        }

        [Fact]
        public async Task ShouldCreateAndGetZones()
        {
            var client = Factory.CreateClient();
            var token = await client.AuthenticateAsAdminUser();
            var locationId = await client.CreateLocation(TestData.Locations.Home, token);
            await client.CreateZone(locationId, TestData.Zones.LivingRoom, token);
            await client.CreateZone(locationId, TestData.Zones.Kitchen, token);

            var zones = await client.GetZones(locationId, token);
            zones.Length.Should().Be(2);
        }

        [Fact]
        public async Task ShouldBeAbleToCreateSameZoneInDifferentLocations()
        {
            var client = Factory.CreateClient();
            var token = await client.AuthenticateAsAdminUser();

            var homeLocationId = await client.CreateLocation(TestData.Locations.Home, token);
            var cabinLocationID = await client.CreateLocation(TestData.Locations.Cabin, token);

            await client.CreateZone(homeLocationId, TestData.Zones.LivingRoom, token);
            await client.CreateZone(cabinLocationID, TestData.Zones.LivingRoom, token);

            var zones = await client.GetZones(homeLocationId, token);
            zones.Length.Should().Be(1);

            zones = await client.GetZones(cabinLocationID, token);
            zones.Length.Should().Be(1);
        }

        [Fact]
        public async Task ShouldHandleCreatingDuplicateZones()
        {
            var client = Factory.CreateClient();
            var token = await client.AuthenticateAsAdminUser();

            var locationId = await client.CreateLocation(TestData.Locations.Home, token);

            await client.CreateZone(locationId, TestData.Zones.LivingRoom, token);
            await client.CreateZone(locationId, TestData.Zones.LivingRoom, token, problem: details => details.ShouldHaveConflictStatus());
        }

        [Fact]
        public async Task ShouldSetZoneAsDefaultOutsideZone()
        {
            var client = Factory.CreateClient();
            var token = await client.AuthenticateAsAdminUser();

            var locationId = await client.CreateLocation(TestData.Locations.Home, token);
            await client.CreateZone(locationId, TestData.Zones.Outside, token);
            await client.CreateZone(locationId, TestData.Zones.LivingRoom, token);

            var zones = await client.GetZones(locationId, token);
            var outsideZone = zones.Single(z => z.Name == TestData.Zones.Outside.Name);
            var zoneDetail = await client.GetZoneDetails(outsideZone.Id, token);

            zoneDetail.IsDefaultOutsideZone.Should().Be(true);
        }

        [Fact]
        public async Task ShouldHandleAddingDuplicateUsersToLocation()
        {
            var client = Factory.CreateClient();
            var token = await client.AuthenticateAsAdminUser();

            var locationId = await client.CreateLocation(TestData.Locations.Home, token);
            var userId = await client.CreateUser(TestData.Users.StandardUser, token);

            await client.AddUserToLocation(locationId, new AddUserToLocationCommand(userId), token);
            await client.AddUserToLocation(locationId, new AddUserToLocationCommand(userId), token, problem: details => details.ShouldHaveConflictStatus());
        }

        [Fact]
        public async Task ShouldAddUserToLocation()
        {
            var client = Factory.CreateClient();
            var token = await client.AuthenticateAsAdminUser();

            var locationId = await client.CreateLocation(TestData.Locations.Home, token);
            var userId = await client.CreateUser(TestData.Users.StandardUser, token);

            await client.AddUserToLocation(locationId, new AddUserToLocationCommand(userId), token);

            var locationUsers = await client.GetUsersByLocation(locationId, token);
            locationUsers.Length.Should().Be(2);
        }

        [Fact]
        public async Task ShouldRemoveUserFromLocation()
        {
            var client = Factory.CreateClient();
            var token = await client.AuthenticateAsAdminUser();

            var locationId = await client.CreateLocation(TestData.Locations.Home, token);
            var userId = await client.CreateUser(TestData.Users.StandardUser, token);

            await client.AddUserToLocation(locationId, new AddUserToLocationCommand(userId), token);
            await client.RemoveUserFromLocation(locationId, userId, token);

            var locationUsers = await client.GetUsersByLocation(locationId, token);
            locationUsers.Length.Should().Be(1);
        }

        [Fact]
        public async Task ShouldDeleteLocation()
        {
            var client = Factory.CreateClient();
            var token = await client.AuthenticateAsAdminUser();

            var locationId = await client.CreateLocation(TestData.Locations.Home, token);
            await client.CreateZone(locationId, TestData.Zones.Outside, token);

            (await client.GetLocations(token)).Should().NotBeEmpty();

            await client.DeleteLocation(locationId, token);

            (await client.GetLocations(token)).Should().BeEmpty();
        }
    }
}
