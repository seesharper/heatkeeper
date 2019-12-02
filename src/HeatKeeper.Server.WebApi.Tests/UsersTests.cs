
using FluentAssertions;
using HeatKeeper.Server.Host.Users;
using HeatKeeper.Server.Users;
using HeatKeeper.Server.Database;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc;

namespace HeatKeeper.Server.WebApi.Tests
{
    public class UsersTests : TestBase
    {
        [Fact]
        public async Task ShouldAuthenticateAdminUser()
        {
            var client = Factory.CreateClient();
            var response = await client.AuthenticateUser(TestData.AuthenticateAdminUserRequest);
            var content = await response.ContentAs<AuthenticatedUserQueryResult>();

            content.IsAdmin.Should().BeTrue();
            content.FirstName.Should().Be(AdminUser.DefaultFirstName);
            content.LastName.Should().Be(AdminUser.DefaultLastName);
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
            var token = await client.AuthenticateAsAdminUser();

            await client.RegisterUser(TestData.Users.StandardUser, token);
            await client.RegisterUser(TestData.Users.AnotherStandardUser, token);

            var response = await client.GetAllUsers();
            var users = await response.ContentAs<User[]>();

            users.Where(u => u.Email != AdminUser.DefaultEmail).Count().Should().Be(2);
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
               .AddContent(new JsonContent(new ChangePasswordCommand(AdminUser.DefaultPassword, NewPassword, NewPassword)))
               .Build();

            var responseMessage = await client.SendAsync(request);
            responseMessage.EnsureSuccessStatusCode();

            var authenticateResponse = await client.PostAuthenticateRequest(AdminUser.DefaultEmail, NewPassword);
            authenticateResponse.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task ShouldUpdateUser()
        {
            var client = Factory.CreateClient();
            var token = await client.AuthenticateAsAdminUser();
            var registerUserResponseMessage = await client.RegisterUser(TestData.Users.StandardUser, token);
            var registerUserResponse = await registerUserResponseMessage.ContentAs<RegisterUserResponse>();

            var updateCommand = new UpdateUserCommand()
            {
                FirstName = TestData.Users.StandardUser.FirstName,
                LastName = TestData.Users.StandardUser.LastName,
                Email = TestData.Users.StandardUser.Email,
                IsAdmin = true
            };

            var response = await client.PatchUser(updateCommand, registerUserResponse.Id, token);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task ShouldUpdateCurrentUser()
        {
            var client = Factory.CreateClient();
            var token = await client.CreateAndAuthenticateStandardUser();
            var updateCommand = new UpdateCurrentUserCommand()
            {
                FirstName = TestData.Users.StandardUser.FirstName,
                LastName = TestData.Users.StandardUser.LastName,
                Email = TestData.Users.StandardUser.Email,
            };

            var response = await client.PatchCurrentUser(updateCommand, token);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Theory]
        [InlineData("PASSWORD", "Password should contain at least one lower case letter")]
        [InlineData("password", "Password should contain at least one upper case letter")]
        [InlineData("Pwd", "Password should not be less than 8 or greater than 64 characters")]
        [InlineData("Password", "Password should contain at least one numeric value")]
        [InlineData("Password1", "Password should contain at least one special case characters")]
        public async Task ShouldEnforcePasswordPoliciesForNewUsers(string password, string errorMessage)
        {
            var client = Factory.CreateClient();
            var token = await client.AuthenticateAsAdminUser();

            var responseMessage = await client.RegisterUser(TestData.Users.StandardUserWithGivenPassword(password), token);

            responseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var problemDetails = await responseMessage.ContentAs<ValidationProblemDetails>();
            problemDetails.Errors.Single().Value.Single().Should().Be(errorMessage);
        }

        [Theory]
        [InlineData("PASSWORD", "Password should contain at least one lower case letter")]
        [InlineData("password", "Password should contain at least one upper case letter")]
        [InlineData("Pwd", "Password should not be less than 8 or greater than 64 characters")]
        [InlineData("Password", "Password should contain at least one numeric value")]
        [InlineData("Password1", "Password should contain at least one special case characters")]
        public async Task ShouldEnforcePasswordPoliciesWhenChangingPassword(string password, string errorMessage)
        {
            var client = Factory.CreateClient();
            var token = await client.AuthenticateAsAdminUser();
            var changePasswordCommand = new ChangePasswordCommand(AdminUser.DefaultPassword, password, password);

            var responseMessage = await client.ChangePassword(changePasswordCommand, token);

            responseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var problemDetails = await responseMessage.ContentAs<ValidationProblemDetails>();
            problemDetails.Errors.Single().Value.Single().Should().Be(errorMessage);
        }
    }
}
