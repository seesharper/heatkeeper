
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using HeatKeeper.Server.Authentication;
using HeatKeeper.Server.Database;
using HeatKeeper.Server.Host.Users;
using HeatKeeper.Server.Users;
using Microsoft.AspNetCore.Mvc;
using Xunit;

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

            await client.PostUser(TestData.Users.StandardUser, token);
            await client.PostUser(TestData.Users.AnotherStandardUser, token);

            var allUsersResponse = await client.GetAllUsers(token);
            var users = await allUsersResponse.ContentAs<User[]>();

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
            var apiKey = await responseMessage.ContentAs<ApiKey>();
            responseMessage.EnsureSuccessStatusCode();
            apiKey.Token.Should().NotBeEmpty();
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
            var userId = await client.PostUser(TestData.Users.StandardUser, token);

            var updateCommand = new UpdateUserCommand()
            {
                UserId = userId,
                FirstName = TestData.Users.StandardUser.FirstName,
                LastName = TestData.Users.StandardUser.LastName,
                Email = TestData.Users.StandardUser.Email,
                IsAdmin = true
            };

            var response = await client.PatchUser(updateCommand, token);
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

            await client.PostUser(TestData.Users.StandardUserWithGivenPassword(password), token, async response =>
            {
                response.StatusCode.ShouldBeBadRequest();
                var problemDetails = await response.ContentAs<ProblemDetails>();
                problemDetails.Detail.Should().Be(errorMessage);
            });
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
            var problemDetails = await responseMessage.ContentAs<ProblemDetails>();
            problemDetails.Detail.Should().Be(errorMessage);
        }

        [Fact]
        public async Task ShouldEnforceValidEmail()
        {
            var client = Factory.CreateClient();
            var token = await client.AuthenticateAsAdminUser();

            client.PostUser(TestData.Users.StandardUserWithInvalidEmail, token, async response =>
            {
                response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
                var problemDetails = await response.ContentAs<ProblemDetails>();
                problemDetails.Detail.Should().Be("The mail address 'InvalidMailAddress' is not correctly formatted.");
            }).Wait();


        }

        [Fact]
        public async Task ShouldCreateAndDeleteUser()
        {
            var client = Factory.CreateClient();
            var token = await client.AuthenticateAsAdminUser();

            var userId = await client.PostUser(TestData.Users.StandardUser, token);

            var allUsersResponse = await client.GetAllUsers(token);
            var allUsers = await allUsersResponse.ContentAs<User[]>();

            allUsers.Should().Contain(u => u.Email == TestData.Users.StandardUser.Email);

            var deleteUserResponse = await client.DeleteUser(new DeleteUserCommand() { UserId = userId }, token);
            deleteUserResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            allUsersResponse = await client.GetAllUsers(token);
            allUsers = await allUsersResponse.ContentAs<User[]>();

            allUsers.Should().NotContain(u => u.Email == TestData.Users.StandardUser.Email);
        }

        [Fact]
        public async Task ShouldNotDeleteCurrentUser()
        {
            var client = Factory.CreateClient();
            var token = await client.AuthenticateAsAdminUser();

            var allUsersResponse = await client.GetAllUsers(token);
            var allUsers = await allUsersResponse.ContentAs<User[]>();

            var adminUser = allUsers.Single(u => u.Email == AdminUser.DefaultEmail);

            var deleteUserResponse = await client.DeleteUser(new DeleteUserCommand() { UserId = adminUser.Id }, token);
            deleteUserResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task ShouldNotChangeAdminStatusForCurrentUser()
        {
            var client = Factory.CreateClient();
            var token = await client.AuthenticateAsAdminUser();

            var allUsersResponse = await client.GetAllUsers(token);
            var allUsers = await allUsersResponse.ContentAs<User[]>();

            var adminUser = allUsers.Single(u => u.Email == AdminUser.DefaultEmail);

            var updateCommand = new UpdateUserCommand()
            {
                UserId = adminUser.Id,
                FirstName = AdminUser.DefaultFirstName,
                LastName = AdminUser.DefaultLastName,
                Email = AdminUser.DefaultEmail,
                IsAdmin = false
            };

            var response = await client.PatchUser(updateCommand, token);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        }
    }
}
