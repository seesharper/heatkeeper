using HeatKeeper.Server.Host.Locations;
using HeatKeeper.Server.Host.Users;

namespace HeatKeeper.Server.WebApi.Tests
{
    public static class TestRequests
    {
        public static RegisterUserRequest CreateTestUserRequest = new RegisterUserRequest("TestUser", "TestUser@gmail.com", false, "TestUser1234", "TestUser1234");

        public static RegisterUserRequest CreateAdminUserRequest = new RegisterUserRequest("AdminUser", "AdminUser@gmail.com", true, "TestUser1234", "TestUser1234");

        public static CreateLocationRequest CreateTestLocationRequest = new CreateLocationRequest("TestLocation", "A test location");
    }

}