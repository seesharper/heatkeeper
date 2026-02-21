using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using HeatKeeper.Server.Locations.Api;
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
            var createdLocation = await client.GetLocationDetails(locationId, token);

            createdLocation.Id.Should().Be(locationId);
            createdLocation.Name.Should().Be(TestData.Locations.Home.Name);
            createdLocation.Description.Should().Be(TestData.Locations.Home.Description);
            createdLocation.Longitude.Should().Be(TestData.Locations.Home.Longitude);
            createdLocation.Latitude.Should().Be(TestData.Locations.Home.Latitude);
        }

        [Fact]
        public async Task ShouldUpdateLocation()
        {
            var client = Factory.CreateClient();
            var token = await client.AuthenticateAsAdminUser();

            var locationId = await client.CreateLocation(TestData.Locations.Home, token);

            var updateLocationCommand = new UpdateLocationCommand(locationId, TestData.Locations.Cabin.Name, TestData.Locations.Cabin.Description, null, null, TestData.Locations.Cabin.Longitude, TestData.Locations.Cabin.Latitude, 0, false, null);


            await client.UpdateLocation(updateLocationCommand, locationId, token);

            var updatedLocation = await client.GetLocationDetails(locationId, token);

            updatedLocation.Name.Should().Be(TestData.Locations.Cabin.Name);
            updatedLocation.Description.Should().Be(TestData.Locations.Cabin.Description);
            updatedLocation.Longitude.Should().Be(TestData.Locations.Cabin.Longitude);
            updatedLocation.Latitude.Should().Be(TestData.Locations.Cabin.Latitude);
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

            var updateLocationCommand = new UpdateLocationCommand(cabinLocationId, TestData.Locations.Home.Name, TestData.Locations.Home.Description, null, null, TestData.Locations.Home.Longitude, TestData.Locations.Home.Latitude, 0, false, null);


            await client.UpdateLocation(updateLocationCommand, cabinLocationId, token, problem: details => details.ShouldHaveConflictStatus());
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



        // [Fact]
        // public async Task ShouldHandleAddingDuplicateUsersToLocation()
        // {
        //     var client = Factory.CreateClient();
        //     var token = await client.AuthenticateAsAdminUser();

        //     var locationId = await client.CreateLocation(TestData.Locations.Home, token);
        //     var userId = await client.CreateUser(TestData.Users.StandardUser, token);

        //     await client.AddUserToLocation(locationId, new AddUserToLocationCommand(userId), token);
        //     await client.AddUserToLocation(locationId, new AddUserToLocationCommand(userId), token, problem: details => details.ShouldHaveConflictStatus());
        // }

        // [Fact]
        // public async Task ShouldAddUserToLocation()
        // {
        //     var client = Factory.CreateClient();
        //     var token = await client.AuthenticateAsAdminUser();

        //     var locationId = await client.CreateLocation(TestData.Locations.Home, token);
        //     var userId = await client.CreateUser(TestData.Users.StandardUser, token);

        //     await client.AddUserToLocation(locationId, new AddUserToLocationCommand(userId), token);

        //     var locationUsers = await client.GetUsersByLocation(locationId, token);
        //     locationUsers.Length.Should().Be(2);
        // }

        // [Fact]
        // public async Task ShouldRemoveUserFromLocation()
        // {
        //     var client = Factory.CreateClient();
        //     var token = await client.AuthenticateAsAdminUser();

        //     var locationId = await client.CreateLocation(TestData.Locations.Home, token);
        //     var userId = await client.CreateUser(TestData.Users.StandardUser, token);

        //     await client.AddUserToLocation(locationId, new AddUserToLocationCommand(userId), token);
        //     await client.RemoveUserFromLocation(locationId, userId, token);

        //     var locationUsers = await client.GetUsersByLocation(locationId, token);
        //     locationUsers.Length.Should().Be(1);
        // }

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

        [Fact]
        public async Task ShouldGetLocationDetails()
        {
            var testLocation = await Factory.CreateTestLocation();
            var client = Factory.CreateClient();
            var locationDetails = await client.GetLocationDetails(testLocation.LocationId, testLocation.Token);
            locationDetails.Name.Should().Be(TestData.Locations.Home.Name);
            locationDetails.Description.Should().Be(TestData.Locations.Home.Description);
            locationDetails.DefaultInsideZoneId.Should().Be(testLocation.LivingRoomZoneId);
        }

        [Fact]
        public async Task ShouldSetAndGetFixedEnergyPrice()
        {
            var client = Factory.CreateClient();
            var token = await client.AuthenticateAsAdminUser();

            var locationId = await client.CreateLocation(TestData.Locations.Home, token);

            var locationDetails = await client.GetLocationDetails(locationId, token);
            locationDetails.FixedEnergyPrice.Should().Be(0);
            locationDetails.UseFixedEnergyPrice.Should().BeFalse();

            var updateCommand = new UpdateLocationCommand(locationId, TestData.Locations.Home.Name, TestData.Locations.Home.Description, null, null, TestData.Locations.Home.Longitude, TestData.Locations.Home.Latitude, 1.25, true, null);
            await client.UpdateLocation(updateCommand, locationId, token);

            locationDetails = await client.GetLocationDetails(locationId, token);
            locationDetails.FixedEnergyPrice.Should().Be(1.25);
            locationDetails.UseFixedEnergyPrice.Should().BeTrue();
        }

        [Fact]
        public async Task ShouldGetLocationTemperatures()
        {
            var testLocation = await Factory.CreateTestLocation();
            var client = Factory.CreateClient();
            var locationTemperatures = await client.GetLocationTemperatures(testLocation.LocationId, testLocation.Token);
            locationTemperatures.Length.Should().Be(1);
            locationTemperatures[0].Name.Should().Be(TestData.Zones.LivingRoom.Name);
            locationTemperatures[0].Temperature.Should().Be(TestData.Measurements.LivingRoomTemperatureMeasurement.Value);
            locationTemperatures[0].Humidity.Should().Be(TestData.Measurements.LivingRoomHumidityMeasurement.Value);
        }
    }
}
