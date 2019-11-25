using HeatKeeper.Server.Database;
using HeatKeeper.Server.Host.Users;
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
    }
}
