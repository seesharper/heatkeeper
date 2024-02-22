using System;
using HeatKeeper.Server.Database;
using HeatKeeper.Server.Locations;
using HeatKeeper.Server.Measurements;
using HeatKeeper.Server.Programs;
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

        public static MeasurementCommand[] TemperatureMeasurementRequests =>
            new[]
                {
                    Measurements.LivingRoomTemperatureMeasurement,
                    Measurements.LivingRoomHumidityMeasurement,
                    Measurements.OutsideTemperatureMeasurement,
                };

        public static MeasurementCommand[] TemperatureMeasurementRequestsWithRetentionPolicy =>
            new[]
                {
                    Measurements.LivingRoomTemperatureWithHourRetentionPolicy,
                    Measurements.LivingRoomTemperatureWithDayRetentionPolicy,
                    Measurements.LivingRoomTemperatureWithWeekRetentionPolicy,
                };

        public static MeasurementCommand[] CumulativeMeasurementsRequests =>
            new[]
            {
                Measurements.CumulativePowerImportMeasurement1,
                Measurements.CumulativePowerImportMeasurement2
            };


        public static class Measurements
        {
            public static MeasurementCommand LivingRoomTemperatureMeasurement => new MeasurementCommand(Sensors.LivingRoomSensor, MeasurementType.Temperature, RetentionPolicy.None, 23.7, DateTime.UtcNow);
            public static MeasurementCommand LivingRoomHumidityMeasurement => new MeasurementCommand(Sensors.LivingRoomSensor, MeasurementType.Humidity, RetentionPolicy.None, 39.3, DateTime.UtcNow);
            public static MeasurementCommand OutsideTemperatureMeasurement => new MeasurementCommand(Sensors.OutsideSensor, MeasurementType.Temperature, RetentionPolicy.None, 10.2, DateTime.UtcNow);
            public static MeasurementCommand LivingRoomTemperatureWithHourRetentionPolicy => new MeasurementCommand(Sensors.LivingRoomSensor, MeasurementType.Temperature, RetentionPolicy.Hour, 23.7, DateTime.UtcNow);
            public static MeasurementCommand LivingRoomTemperatureWithDayRetentionPolicy => new MeasurementCommand(Sensors.LivingRoomSensor, MeasurementType.Temperature, RetentionPolicy.Day, 23.7, DateTime.UtcNow);
            public static MeasurementCommand LivingRoomTemperatureWithWeekRetentionPolicy => new MeasurementCommand(Sensors.LivingRoomSensor, MeasurementType.Temperature, RetentionPolicy.Week, 23.7, DateTime.UtcNow);
            public static MeasurementCommand CumulativePowerImportMeasurement1 = new MeasurementCommand(Sensors.PowerMeter, MeasurementType.CumulativePowerImport, RetentionPolicy.None, 150000, DateTime.UtcNow.AddHours(-1));
            public static MeasurementCommand CumulativePowerImportMeasurement2 = new MeasurementCommand(Sensors.PowerMeter, MeasurementType.CumulativePowerImport, RetentionPolicy.None, 150000, DateTime.UtcNow);
        }

        public static class Locations
        {
            public static CreateLocationCommand Home =>
                new CreateLocationCommand() { Name = "Home", Description = "Description of the Home location" };

            public static CreateLocationCommand Cabin =>
                new CreateLocationCommand() { Name = "Cabin", Description = "Description of the Cabin location" };
        }

        public static class Zones
        {
            public const string LivingRoomName = "LivingRoom";
            public const string LivingRoomDescription = "This is the description of the LivingRoom zone";
            public const bool LivingRoomIsDefaultInsideZone = true;
            public const bool LivingRoomIsDefaultOutsideZone = false;
            public const string LivingRoomMqttTopic = "LivingRoomTopic";

            public const string KitchenName = "Kitchen";
            public const string KitchenDescription = "This is the description of the Kitchen zone";
            public const bool KitchenIsDefaultInsideZone = false;
            public const bool KitchenIsDefaultOutsideZone = false;
            public const string KitchenMqttTopic = "KitchenTopic";

            public const string OutsideName = "Outside";
            public const string OutsideDescription = "This is the description of the outside zone";
            public const bool OutsideIsDefaultInsideZone = false;
            public const bool OutsideIsDefaultOutsideZone = true;

            public const string PowerMeterName = "PowerMeter";
            public const string PowerMeterDescription = "This is the description of the PowerMeter zone";

            public const string TestZoneName = "TestZone";
            public const string TestZoneDescription = "This is the description of the TestZone zone";
            public const string TestZoneMqttTopic = "TestTopic";
            public const bool TestZoneIsDefaultInsideZone = false;
            public const bool TestZoneIsDefaultOutsideZone = false;

            public static CreateZoneCommand LivingRoom =>
                new() { Name = LivingRoomName, Description = LivingRoomDescription, IsDefaultInsideZone = LivingRoomIsDefaultInsideZone, MqttTopic = LivingRoomMqttTopic };

            public static CreateZoneCommand Outside =>
                new() { Name = OutsideName, Description = OutsideDescription, IsDefaultOutsideZone = OutsideIsDefaultOutsideZone, IsDefaultInsideZone = OutsideIsDefaultInsideZone };

            public static CreateZoneCommand Kitchen =>
                new() { Name = KitchenName, Description = KitchenDescription, MqttTopic = KitchenMqttTopic };

            public static CreateZoneCommand PowerMeter =>
                new() { Name = PowerMeterName, Description = PowerMeterDescription };

            public static CreateZoneCommand TestZone =>
                new() { Name = TestZoneName, Description = TestZoneDescription, MqttTopic = TestZoneMqttTopic, IsDefaultInsideZone = TestZoneIsDefaultInsideZone, IsDefaultOutsideZone = TestZoneIsDefaultOutsideZone };

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
        public static class Sensors
        {
            public static string LivingRoomSensor = "SensorID1";

            public static string OutsideSensor = "SensorID2";

            public static string PowerMeter = "PM1234";
        }

        public static class Programs
        {

            public const string NormalProgramName = "Normal";
            public const string NormalProgramDescription = "Description of normal";
            public const string AwayProgramName = "Away";
            public const string AwayProgramDescription = "Description of away";
            public const string TestProgramName = "TestProgram";
            public const string TestProgramDescription = "TestProgramDescription";
            public const string TestProgramUpdatedName = "TestProgramUpdated";
            public const string TestProgramUpdatedDescription = "TestProgramUpdatedDescription";


            public static CreateProgramCommand Normal(long locationId) =>
                new(NormalProgramName, NormalProgramDescription, locationId);

            public static CreateProgramCommand Away(long locationId) =>
                new(AwayProgramName, AwayProgramDescription, locationId);

            public static CreateProgramCommand TestProgram(long locationId) =>
                new(TestProgramName, TestProgramDescription, locationId);

            public static UpdateProgramCommand UpdatedTestProgram(long programId, long scheduleId) =>
                new(programId, TestProgramUpdatedName, TestProgramUpdatedDescription, scheduleId);
        }

        public static class Schedules
        {
            public const string DayTimeScheduleName = "DayTime";
            public const string DayTimeScheduleCronExpression = "0 15,18, 21 * * *";
            public const string TestScheduleName = "TestSchedule";
            public const string TestScheduleCronExpression = "0 15,18,21 * * *";
            public const string TestScheduleUpdatedName = "TestScheduleUpdated";
            public const string TestScheduleUpdatedCronExpression = "0 20,18,21 * * *";

            public static CreateScheduleCommand DayTime(long programId) =>
                new(programId, DayTimeScheduleName, DayTimeScheduleCronExpression);

            public static CreateScheduleCommand TestSchedule(long programId) =>
                 new(programId, TestScheduleName, TestScheduleCronExpression);

            public static UpdateScheduleCommand UpdatedSchedule(long scheduleId) =>
                new(scheduleId, TestScheduleUpdatedName, TestScheduleUpdatedCronExpression);
        }

        public static class SetPoints
        {
            public const double LivingRoomSetPoint = 20;
            public const double LivingRoomHysteresis = 2;

            public const double UpdatedLivingRoomSetPoint = 21;
            public const double UpdatedLivingRoomHysteresis = 3;


            public static CreateSetPointCommand LivingRoom(long scheduleId, long livingRoomZoneId) =>
                new(scheduleId, livingRoomZoneId, LivingRoomSetPoint, LivingRoomHysteresis);

            public static UpdateSetPointCommand UpdatedLivingRoom(long setPointId) =>
                new(setPointId, UpdatedLivingRoomSetPoint, UpdatedLivingRoomHysteresis);
        }


    }


}
