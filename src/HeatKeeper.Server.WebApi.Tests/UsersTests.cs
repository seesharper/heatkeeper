using AutoFixture;
using FluentAssertions;
using HeatKeeper.Server.Host.Users;
using HeatKeeper.Server.Users;
using HeatKeeper.Server.Database;
using HeatKeeper.Server.WebApi.Tests.Customizations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HeatKeeper.Server.WebApi.Tests
{
    public class UsersTests : TestBase
    {
        public UsersTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            Fixture.Customizations.Add(new MailAddressCustomization());
            Fixture.Customizations.Add(new PasswordCustomization());
        }

        [Fact]
        public async Task ShouldAuthenticateAdminUser()
        {
            var client = Factory.CreateClient();
            var response = await client.AuthenticateUser(TestData.AuthenticateAdminUserRequest);
            var content = await response.ContentAs<AuthenticateUserResponse>();

            content.IsAdmin.Should().BeTrue();
            content.Name.Should().Be(AdminUser.UserName);
            content.Email.Should().Be(AdminUser.DefaultEmail);
            content.Token.Should().NotBeEmpty();
        }

        [Fact]
        public async Task ShouldNotAuthenticateAdminUserWithInvalidPassword()
        {
            var client = Factory.CreateClient();
            var response = await client.AuthenticateUser(TestData.InvalidAuthenticateAdminUserRequest);
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task ShouldCreateAndGetAllUsers()
        {
            var client = Factory.CreateClient();

            await client.CreateUser(Fixture.Create<RegisterUserRequest>());
            await client.CreateUser(Fixture.Create<RegisterUserRequest>());
            await client.CreateUser(Fixture.Create<RegisterUserRequest>());

            var response = await client.GetAllUsers();
            var users = await response.ContentAs<GetUserResponse[]>();

            users.Where(u => u.Email != AdminUser.DefaultEmail).Count().Should().Be(3);
        }

        [Fact]
        public async Task ShouldCreateApiKey()
        {
            var client = Factory.CreateClient();
            var token = await client.AuthenticateAsAdminUser();

            var request = new HttpRequestBuilder()
                .WithMethod(HttpMethod.Get)
                .AddRequestUri("api/users/apikey")
                .AddBearerToken(token)
                .Build();

            var responseMessage = await client.SendAsync(request);
            responseMessage.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task ShouldChangePasswordAndAuthenticate()
        {
            const string NewPassword = "^SzCzWW5D@EaU8veHkJaRqlY";
            var client = Factory.CreateClient();
            var token = await client.AuthenticateAsAdminUser();

            var request = new HttpRequestBuilder()
               .WithMethod(HttpMethod.Patch)
               .AddRequestUri("api/users/password")
               .AddBearerToken(token)
               .AddContent(new JsonContent(new ChangePasswordRequest(AdminUser.DefaultPassword, NewPassword, NewPassword)))
               .Build();

            var responseMessage = await client.SendAsync(request);
            responseMessage.EnsureSuccessStatusCode();

            var authenticateResponse = await client.PostAuthenticateRequest(AdminUser.UserName, NewPassword);
            authenticateResponse.EnsureSuccessStatusCode();
        }
    }
}
