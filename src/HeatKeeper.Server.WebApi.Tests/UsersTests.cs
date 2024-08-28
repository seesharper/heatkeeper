
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CQRS.AspNet.Testing;
using FluentAssertions;
using HeatKeeper.Server.Database;
using HeatKeeper.Server.Users;
using HeatKeeper.Server.Users.Api;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Mvc.Testing.Handlers;
using Microsoft.Net.Http.Headers;
using Xunit;

namespace HeatKeeper.Server.WebApi.Tests;

public class UsersTests : TestBase
{
    [Fact]
    public async Task ShouldAuthenticateAdminUser()
    {
        var client = Factory.CreateClient();
        var authenticatedUser = await client.GetAuthenticatedUser(TestData.AuthenticateAdminUserRequest, success: response => response.StatusCode.ShouldBeOK());

        authenticatedUser.IsAdmin.Should().BeTrue();
        authenticatedUser.FirstName.Should().Be(AdminUser.DefaultFirstName);
        authenticatedUser.LastName.Should().Be(AdminUser.DefaultLastName);
        authenticatedUser.Email.Should().Be(AdminUser.DefaultEmail);
        authenticatedUser.Token.Should().NotBeEmpty();
    }

    [Fact]
    public async Task ShouldGetUserDetails()
    {
        var client = Factory.CreateClient();
        var testLocation = await Factory.CreateTestLocation();

        var users = await client.GetAllUsers(testLocation.Token);
        var userDetails = await client.GetUserDetails(users.First().Id, testLocation.Token);

        userDetails.Email.Should().Be(users.First().Email);
        userDetails.FirstName.Should().Be(users.First().FirstName);
        userDetails.LastName.Should().Be(users.First().LastName);
        userDetails.IsAdmin.Should().Be(users.First().IsAdmin);

    }

    [Fact]
    public async Task ShouldGetUserLocationAccess()
    {
        var client = Factory.CreateClient();
        var testLocation = await Factory.CreateTestLocation();

        var users = await client.GetAllUsers(testLocation.Token);
        var userDetails = await client.GetUserDetails(users.First().Id, testLocation.Token);

        var userLocationsAccess = await client.GetUserLocationsAccess(users.First().Id, testLocation.Token);
        userLocationsAccess.Single().HasAccess.Should().BeTrue();
        userLocationsAccess.Should().Contain(l => l.LocationId == testLocation.LocationId);
    }

    [Fact]
    public async Task ShouldAssignLocationToUser()
    {
        var client = Factory.CreateClient();
        var testLocation = await Factory.CreateTestLocation();
        var testUserId = await client.CreateUser(TestData.Users.StandardUser, testLocation.Token);

        var userLocationsAccess = await client.GetUserLocationsAccess(testUserId, testLocation.Token);
        userLocationsAccess.Single().HasAccess.Should().BeFalse();
        await client.AssignLocationToUser(new AssignLocationToUserCommand(testUserId, testLocation.LocationId), testLocation.Token);
        userLocationsAccess = await client.GetUserLocationsAccess(testUserId, testLocation.Token);
        userLocationsAccess.Single().HasAccess.Should().BeTrue();
    }

    [Fact]
    public async Task ShouldRemoveLocationFromUser()
    {
        var client = Factory.CreateClient();
        var testLocation = await Factory.CreateTestLocation();
        var testUserId = await client.CreateUser(TestData.Users.StandardUser, testLocation.Token);

        var userLocationsAccess = await client.GetUserLocationsAccess(testUserId, testLocation.Token);
        userLocationsAccess.Single().HasAccess.Should().BeFalse();
        await client.AssignLocationToUser(new AssignLocationToUserCommand(testUserId, testLocation.LocationId), testLocation.Token);
        userLocationsAccess = await client.GetUserLocationsAccess(testUserId, testLocation.Token);
        userLocationsAccess.Single().HasAccess.Should().BeTrue();

        await client.RemoveLocationFromUser(new RemoveLocationFromUserCommand(testUserId, testLocation.LocationId), testLocation.Token);
        userLocationsAccess = await client.GetUserLocationsAccess(testUserId, testLocation.Token);
        userLocationsAccess.Single().HasAccess.Should().BeFalse();
    }




    [Fact]
    public async Task ShouldNotAuthenticateAdminUserWithInvalidPassword()
    {
        var client = Factory.CreateClient();
        var response = await client.GetAuthenticatedUser(
            TestData.InvalidAuthenticateAdminUserRequest,
            success: response => response.StatusCode.ShouldBeOK(),
            problem: details => details.ShouldHaveUnauthorizedStatus());
    }

    [Fact]
    public async Task ShouldCreateAndGetAllUsers()
    {
        var client = Factory.CreateClient();
        var token = await client.AuthenticateAsAdminUser();

        await client.CreateUser(TestData.Users.StandardUser, token);
        await client.CreateUser(TestData.Users.AnotherStandardUser, token);

        var users = await client.GetAllUsers(token);
        users.Where(u => u.Email != AdminUser.DefaultEmail).Count().Should().Be(2);
    }

    [Fact]
    public async Task ShouldGetApiKey()
    {
        var client = Factory.CreateClient();
        var token = await client.AuthenticateAsAdminUser();

        var apiKey = await client.GetApiKey(token);

        apiKey.Token.Should().NotBeEmpty();

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = (JwtSecurityToken)handler.ReadToken(apiKey.Token);

        var exp = jwtToken.Claims.First(claim => claim.Type == "exp").Value;

        var expirationDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(exp));

        expirationDate.Year.Should().Be(DateTime.UtcNow.Year + 10);
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
        var userId = await client.CreateUser(TestData.Users.StandardUser, token);

        var updateCommand = new UpdateUserCommand(userId, TestData.Users.StandardUser.Email,TestData.Users.StandardUser.FirstName,TestData.Users.StandardUser.LastName,  true);


        await client.UpdateUser(updateCommand, token);
    }

    [Fact]
    public async Task ShouldUpdateCurrentUser()
    {
        var client = Factory.CreateClient();
        var token = await client.CreateAndAuthenticateStandardUser();
        var updateCommand = new UpdateCurrentUserCommand(Email: TestData.Users.StandardUser.Email, FirstName: TestData.Users.StandardUser.FirstName, LastName: TestData.Users.StandardUser.LastName);
        await client.UpdateCurrentUser(updateCommand, token);
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

        await client.CreateUser(TestData.Users.StandardUserWithGivenPassword(password), token, problem: details =>
        {
            details.ShouldHaveBadRequestStatus();
            details.Detail.Should().Be(errorMessage);
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

        await client.ChangePassword(changePasswordCommand, token, problem: details =>
        {
            details.ShouldHaveBadRequestStatus();
            details.Detail.Should().Be(errorMessage);
        });
    }

    [Fact]
    public async Task ShouldEnforceValidEmail()
    {
        var client = Factory.CreateClient();
        var token = await client.AuthenticateAsAdminUser();

        await client.CreateUser(TestData.Users.StandardUserWithInvalidEmail, token, problem: details =>
       {
           details.ShouldHaveBadRequestStatus();
           details.Detail.Should().Be("The mail address 'InvalidMailAddress' is not correctly formatted.");
       });
    }

    [Fact]
    public async Task ShouldCreateAndDeleteUser()
    {
        var client = Factory.CreateClient();
        var token = await client.AuthenticateAsAdminUser();

        var userId = await client.CreateUser(TestData.Users.StandardUser, token);

        var allUsers = await client.GetAllUsers(token);
        allUsers.Should().Contain(u => u.Email == TestData.Users.StandardUser.Email);

        await client.DeleteUser(userId, token);

        allUsers = await client.GetAllUsers(token);
        allUsers.Should().NotContain(u => u.Email == TestData.Users.StandardUser.Email);
    }

    [Fact]
    public async Task ShouldNotDeleteCurrentUser()
    {
        var client = Factory.CreateClient();
        var token = await client.AuthenticateAsAdminUser();

        var allUsers = await client.GetAllUsers(token);
        var adminUser = allUsers.Single(u => u.Email == AdminUser.DefaultEmail);

        await client.DeleteUser(adminUser.Id, token, problem: details => details.ShouldHaveBadRequestStatus());
    }

    [Fact]
    public async Task ShouldNotChangeAdminStatusForCurrentUser()
    {
        var client = Factory.CreateClient();
        var token = await client.AuthenticateAsAdminUser();

        var allUsers = await client.GetAllUsers(token);

        var adminUser = allUsers.Single(u => u.Email == AdminUser.DefaultEmail);

        var updateCommand = new UpdateUserCommand(adminUser.Id, TestData.Users.StandardUser.FirstName, TestData.Users.StandardUser.LastName, TestData.Users.StandardUser.Email, false);


        await client.UpdateUser(updateCommand, token, problem: details => details.ShouldHaveBadRequestStatus());
    }

    // public async Task ShouldValidateUserWhenCreatingUser()
    // {
    //     var client = Factory.CreateClient();
    //     Factory.MockService<IUserValidator>
    //     var testLocation = await Factory.CreateTestLocation();
    //     var token = await client.AuthenticateAsAdminUser();
    //     await client.CreateUser(TestData.Users.StandardUser, token);        
    // }



    [Fact]
    public async Task ShouldRefreshToken()
    {
        var httpRequest = new HttpRequestBuilder()
                .AddJsonContent(TestData.AuthenticateAdminUserRequest)
                .WithMethod(HttpMethod.Post)
                .AddRequestUri("api/users/authenticate")
                .Build();
        var client = Factory.CreateClient();
        var response = await client.SendAsync(httpRequest);
        var cookie = response.Headers.GetValues("Set-Cookie").First();


        var httpRequest2 = new HttpRequestBuilder()
               .WithMethod(HttpMethod.Post)
               .AddRequestUri("api/users/refresh-token")
               .Build();

        httpRequest2.Headers.Add(HeaderNames.Cookie, cookie);

        response = await client.SendAsync(httpRequest2);


    }
}
