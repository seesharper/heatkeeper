using System.Threading.Tasks;
using HeatKeeper.Server.Host.Users;
using HeatKeeper.Server.Users;
using Xunit;
using Xunit.Abstractions;
using FluentAssertions;
using System.Net;

namespace HeatKeeper.Server.WebApi.Tests
{
    public class UsersTests : TestBase
    {
        public UsersTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public async Task ShouldAuthenticateAdminUser()
        {
            var request = new AuthenticateUserRequest(AdminUser.UserName, AdminUser.DefaultPassword);
            var client = Factory.CreateClient();
            var response = await client.PostAsync("Users/authenticate", new JsonContent(request));
            response.EnsureSuccessStatusCode();
        }

        [Fact]
         public async Task ShouldNotAuthenticateAdminUserWithInvalidPassword()
         {
            var request = new AuthenticateUserRequest(AdminUser.UserName, "InvalidPassword");
            var client = Factory.CreateClient();
            var response = await client.PostAsync("Users/authenticate", new JsonContent(request));
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
         }

    }
}