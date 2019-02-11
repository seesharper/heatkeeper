using System.Threading.Tasks;
using HeatKeeper.Server.Host.Users;
using HeatKeeper.Server.Users;
using Xunit;
using Xunit.Abstractions;
using FluentAssertions;
using System.Net;
using System.Net.Http;

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
            var response = await client.PostAsync("api/users/authenticate", new JsonContent(request));
            var content = response.ContentAs<AuthenticateUserResponse>();
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task ShouldNotAuthenticateAdminUserWithInvalidPassword()
        {
            var request = new AuthenticateUserRequest(AdminUser.UserName, "InvalidPassword");
            var client = Factory.CreateClient();
            var response = await client.PostAsync("api/users/authenticate", new JsonContent(request));
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task ShouldCreateUser()
        {
            var client = Factory.CreateClient();

            var token = await client.AuthenticateAsAdminUser();

            var createUserRequest = new CreateUserRequest("TestUser", "TestUser@gmail.com", true, "TestUser12324");
            var postRequest = new HttpRequestBuilder()
                .AddMethod(HttpMethod.Post)
                .AddRequestUri("api/users")
                .AddBearerToken(token)
                .AddContent(new JsonContent(createUserRequest))
                .Build();

            var response = await client.SendAsync(postRequest);
            var c = await response.Content.ReadAsStringAsync();
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task ShouldCreateApiKey()
        {
             var client = Factory.CreateClient();
             var token = await client.AuthenticateAsAdminUser();

            var request = new HttpRequestBuilder()
                .AddMethod(HttpMethod.Get)
                .AddRequestUri("api/users/apikey")
                .AddBearerToken(token)
                .Build();

            var responseMessage = await client.SendAsync(request);
            responseMessage.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task ShouldChangePasswordAndAuthenticate()
        {
             const string newPassword = "^SzCzWW5D@EaU8veHkJaRqlY";
             var client = Factory.CreateClient();
             var token = await client.AuthenticateAsAdminUser();

             var request = new HttpRequestBuilder()
                .AddMethod(HttpMethod.Patch)
                .AddRequestUri("api/users/password")
                .AddBearerToken(token)
                .AddContent(new JsonContent(new ChangePasswordRequest(AdminUser.DefaultPassword, newPassword, newPassword)))
                .Build();

            var responseMessage = await client.SendAsync(request);
            responseMessage.EnsureSuccessStatusCode();

            var authenticateResponse = await client.PostAuthenticateRequest(AdminUser.UserName, newPassword);
            authenticateResponse.EnsureSuccessStatusCode();
        }


    }
}