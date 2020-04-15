using System;
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
        public static string ValidPassword => "aVe78!*PZ9&Lnqh1E4pG";

        public static AuthenticatedUserQuery InvalidAuthenticateAdminUserRequest =>
            new AuthenticatedUserQuery(AdminUser.DefaultEmail, "InvalidPassword");

        public static AuthenticatedUserQuery AuthenticateAdminUserRequest =>
            new AuthenticatedUserQuery(AdminUser.DefaultEmail, AdminUser.DefaultPassword);

        public static CreateMeasurementCommand[] TemperatureMeasurementRequests =>
            new[] { new CreateMeasurementCommand("SensorID1", MeasurementType.Temperature, 23.7, DateTime.UtcNow) };

        public static class Locations
        {
            public static CreateLocationCommand Home =>
                new CreateLocationCommand() { Name = "Home", Description = "Description of the Home location" };

            public static CreateLocationCommand Cabin =>
                new CreateLocationCommand() { Name = "Cabin", Description = "Description of the Cabin location" };
        }

        public static class Zones
        {
            public static CreateZoneCommand LivingRoom =>
                new CreateZoneCommand() { Name = "LivingRoom", Description = "This is the description of the LivingRoom zone", UseAsDefaultInsideZone = true };

            public static CreateZoneCommand Outside =>
                new CreateZoneCommand() { Name = "Outside", Description = "This is the description of the outside zone", UseAsDefaultOutsideZone = true };

            public static CreateZoneCommand Kitchen =>
                new CreateZoneCommand() { Name = "Kitchen", Description = "This is the description of the Kitchen zone" };
        }

        public static class Users
        {
            public static RegisterUserCommand StandardUser =>
                new RegisterUserCommand() { Email = "StandardUser@tempuri.org", FirstName = "FirstName", LastName = "LastName", IsAdmin = false, Password = ValidPassword, ConfirmedPassword = ValidPassword };
            public static RegisterUserCommand AnotherStandardUser =>
                new RegisterUserCommand() { Email = "AnotherStandardUser@tempuri.org", FirstName = "FirstName", LastName = "LastName", IsAdmin = false, Password = ValidPassword, ConfirmedPassword = ValidPassword };

            public static RegisterUserCommand StandardUserWithWeakPassord =>
                new RegisterUserCommand() { Email = "StandardUser@tempuri.org", FirstName = "FirstName", LastName = "LastName", IsAdmin = false, Password = "abc123", ConfirmedPassword = "abc123" };

            public static RegisterUserCommand StandardUserWithGivenPassword(string password) =>
                new RegisterUserCommand() { Email = "StandardUser@tempuri.org", FirstName = "FirstName", LastName = "LastName", IsAdmin = false, Password = password, ConfirmedPassword = password };

            public static RegisterUserCommand StandardUserWithInvalidEmail =>
                new RegisterUserCommand() { Email = "InvalidMailAddress", FirstName = "FirstName", LastName = "LastName", IsAdmin = false, Password = ValidPassword, ConfirmedPassword = ValidPassword };
        }
    }
}
