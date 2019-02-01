using HeatKeeper.Server.Host.Locations;
using HeatKeeper.Server.Host.Users;

namespace HeatKeeper.Server.WebApi.Tests
{
    public static class TestRequests
    {
        public static CreateUserRequest CreateTestUserRequest = new CreateUserRequest("TestUser", "TestUser@gmail.com", false, "TestUser1234");

        public static CreateUserRequest CreateAdminUserRequest = new CreateUserRequest("AdminUser", "AdminUser@gmail.com", true, "TestUser1234");

        public static CreateLocationRequest CreateTestLocationRequest = new CreateLocationRequest("TestLocation", "A test location");
    }

}