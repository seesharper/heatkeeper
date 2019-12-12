using HeatKeeper.Server.Authentication;
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
        public static AuthenticatedUserQuery InvalidAuthenticateAdminUserRequest =>
            new AuthenticatedUserQuery(AdminUser.DefaultEmail, "InvalidPassword");

        public static AuthenticatedUserQuery AuthenticateAdminUserRequest =>
            new AuthenticatedUserQuery(AdminUser.DefaultEmail, AdminUser.DefaultPassword);

        public static CreateMeasurementCommand[] TemperatureMeasurementRequests =>
            new[] { new CreateMeasurementCommand("SensorID1", MeasurementType.Temperature, 23.7) };

        public static RegisterUserCommand RegisterStandardUserRequest =>
            new RegisterUserCommand("StandardUser@tempuri.org", "Standard User First Name", "Standard User Last name", isAdmin: false, "aVe78!*PZ9&Lnqh1E4pG", "aVe78!*PZ9&Lnqh1E4pG");

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
            public static RegisterUserCommand StandardUser =>
                new RegisterUserCommand("StandardUser@tempuri.org", "FirstName", "LastName", isAdmin: false, "aVe78!*PZ9&Lnqh1E4pG", "aVe78!*PZ9&Lnqh1E4pG");
            public static RegisterUserCommand AnotherStandardUser =>
                new RegisterUserCommand("AnotherStandardUser@tempuri.org", "FirstName", "LastName", isAdmin: false, "aVe78!*PZ9&Lnqh1E4pG", "aVe78!*PZ9&Lnqh1E4pG");

            public static RegisterUserCommand StandardUserWithWeakPassord =>
                new RegisterUserCommand("StandardUser@tempuri.org", "FirstName", "LastName", isAdmin: false, "abc123", "abc123");

            public static RegisterUserCommand StandardUserWithGivenPassword(string password) =>
                new RegisterUserCommand("StandardUser@tempuri.org", "FirstName", "LastName", isAdmin: false, password, password);

            public static RegisterUserCommand StandardUserWithInvalidEmail =>
                new RegisterUserCommand("InvalidMailAddress", "FirstName", "LastName", isAdmin: false, "aVe78!*PZ9&Lnqh1E4pG", "aVe78!*PZ9&Lnqh1E4pG");
        }
    }
}
