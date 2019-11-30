using HeatKeeper.Server.Database;
using HeatKeeper.Server.Host.Locations;
using HeatKeeper.Server.Host.Users;
using HeatKeeper.Server.Locations;
using HeatKeeper.Server.Measurements;
using HeatKeeper.Server.Users;
using HeatKeeper.Server.Zones;

namespace HeatKeeper.Server.WebApi.Tests
{
    public static class TestData
    {
        public static AuthenticateUserRequest InvalidAuthenticateAdminUserRequest =>
            new AuthenticateUserRequest(AdminUser.DefaultEmail, "InvalidPassword");

        public static AuthenticateUserRequest AuthenticateAdminUserRequest =>
            new AuthenticateUserRequest(AdminUser.DefaultEmail, AdminUser.DefaultPassword);

        public static CreateMeasurementRequest[] TemperatureMeasurementRequests =>
            new[] { new CreateMeasurementRequest("SensorID1", MeasurementType.Temperature, 23.7) };

        public static RegisterUserRequest RegisterStandardUserRequest =>
            new RegisterUserRequest("StandardUser@tempuri.org", "Standard User First Name", "Standard User Last name", isAdmin: false, "aVe78!*PZ9&Lnqh1E4pG", "aVe78!*PZ9&Lnqh1E4pG");

        public static class Locations
        {
            public static CreateLocationCommand Home =>
                new CreateLocationCommand("Home", "Description of the Home location");

            public static CreateLocationCommand Cabin =>
                new CreateLocationCommand("Cabin", "Description of the Cabin location");
        }

        public static class Zones
        {
            public static CreateZoneCommand LivingRoom =>
                new CreateZoneCommand("LivingRoom", "This is the description of the LivingRoom zone");

            public static CreateZoneCommand Kitchen =>
                new CreateZoneCommand("Kitchen", "This is the description of the Kitchen zone");
        }

        public static class Users
        {
            public static RegisterUserRequest StandardUser =>
                new RegisterUserRequest("StandardUser@tempuri.org", "FirstName", "LastName", isAdmin: false, "aVe78!*PZ9&Lnqh1E4pG", "aVe78!*PZ9&Lnqh1E4pG");
            public static RegisterUserRequest AnotherStandardUser =>
                new RegisterUserRequest("AnotherStandardUser@tempuri.org", "FirstName", "LastName", isAdmin: false, "aVe78!*PZ9&Lnqh1E4pG", "aVe78!*PZ9&Lnqh1E4pG");
        }
    }
}
