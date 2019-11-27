using HeatKeeper.Server.Database;
using HeatKeeper.Server.Host.Locations;
using HeatKeeper.Server.Host.Users;
using HeatKeeper.Server.Host.Zones;
using HeatKeeper.Server.Measurements;
using HeatKeeper.Server.Users;

namespace HeatKeeper.Server.WebApi.Tests
{
    public static class TestData
    {
        public static AuthenticateUserRequest InvalidAuthenticateAdminUserRequest =>
            new AuthenticateUserRequest(AdminUser.UserName, "InvalidPassword");

        public static AuthenticateUserRequest AuthenticateAdminUserRequest =>
            new AuthenticateUserRequest(AdminUser.UserName, AdminUser.DefaultPassword);

        public static CreateMeasurementRequest[] TemperatureMeasurementRequests =>
            new[] { new CreateMeasurementRequest("SensorID1", MeasurementType.Temperature, 23.7) };

        public static RegisterUserRequest RegisterStandardUserRequest =>
            new RegisterUserRequest("StandardUser", "StandardUser@tempuri.org", isAdmin: false, "aVe78!*PZ9&Lnqh1E4pG", "aVe78!*PZ9&Lnqh1E4pG");

        public static class Locations
        {
            public static CreateLocationRequest Home =>
                new CreateLocationRequest("Home", "Description of the Home location");

            public static CreateLocationRequest Cabin =>
                new CreateLocationRequest("Cabin", "Description of the Cabin location");
        }

        public static class Zones
        {
            public static CreateZoneRequest LivingRoom =>
                new CreateZoneRequest("LivingRoom", "This is the description of the LivingRoom zone");

            public static CreateZoneRequest Kitchen =>
                new CreateZoneRequest("Kitchen", "This is the description of the Kitchen zone");
        }

        public static class Users
        {
            public static RegisterUserRequest StandardUser =>
                new RegisterUserRequest("StandardUser", "StandardUser@tempuri.org", isAdmin: false, "aVe78!*PZ9&Lnqh1E4pG", "aVe78!*PZ9&Lnqh1E4pG");
            public static RegisterUserRequest AnotherStandardUser =>
                new RegisterUserRequest("AnotherStandardUser", "AnotherStandardUser@tempuri.org", isAdmin: false, "aVe78!*PZ9&Lnqh1E4pG", "aVe78!*PZ9&Lnqh1E4pG");
        }
    }
}
