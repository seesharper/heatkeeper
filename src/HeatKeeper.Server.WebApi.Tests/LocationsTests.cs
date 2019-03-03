using AutoFixture;
using FluentAssertions;
using HeatKeeper.Server.Host.Locations;
using HeatKeeper.Server.Host.Users;
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