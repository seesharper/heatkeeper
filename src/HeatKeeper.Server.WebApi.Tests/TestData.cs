using System;
using HeatKeeper.Server.Database;
using HeatKeeper.Server.EnergyPriceAreas.Api;
using HeatKeeper.Server.Heaters.Api;
using HeatKeeper.Server.Locations;
using HeatKeeper.Server.Locations.Api;
using HeatKeeper.Server.Measurements;
using HeatKeeper.Server.Mqtt;
using HeatKeeper.Server.Programs;
using HeatKeeper.Server.Programs.Api;
using HeatKeeper.Server.PushSubscriptions.Api;
using HeatKeeper.Server.Schedules.Api;
using HeatKeeper.Server.SetPoints.Api;
using HeatKeeper.Server.Users;
using HeatKeeper.Server.Users.Api;
using HeatKeeper.Server.VATRates;
using HeatKeeper.Server.Zones.Api;

namespace HeatKeeper.Server.WebApi.Tests
{
    public static partial class TestData
    {
        public static string ValidPassword => "aVe78!*PZ9&Lnqh1E4pG";

        public const string InvalidCronExpression = "InvalidCronExpression";

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

        public static MeasurementCommand[] KitchenTemperatureMeasurements =>
            new[]
                {
                    Measurements.LivingRoomTemperatureWithHourRetentionPolicy,
                    Measurements.LivingRoomTemperatureWithDayRetentionPolicy,
                    Measurements.LivingRoomTemperatureWithWeekRetentionPolicy,
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
            public static MeasurementCommand LivingRoomTemperatureMeasurement => new MeasurementCommand(Sensors.LivingRoomSensor, MeasurementType.Temperature, RetentionPolicy.Day, 23.7, Clock.Today);
            public static MeasurementCommand LivingRoomHumidityMeasurement => new MeasurementCommand(Sensors.LivingRoomSensor, MeasurementType.Humidity, RetentionPolicy.Day, 39.3, Clock.Today);

            public static MeasurementCommand KitchenTemperatureMeasurement => new MeasurementCommand(Sensors.KitchenSensor, MeasurementType.Temperature, RetentionPolicy.Day, 23.7, Clock.Today);
            public static MeasurementCommand KitchenHumidityMeasurement => new MeasurementCommand(Sensors.KitchenSensor, MeasurementType.Humidity, RetentionPolicy.Day, 39.3, Clock.Today);
            public static MeasurementCommand OutsideTemperatureMeasurement => new MeasurementCommand(Sensors.OutsideSensor, MeasurementType.Temperature, RetentionPolicy.Day, 10.2, DateTime.UtcNow);
            public static MeasurementCommand LivingRoomTemperatureWithHourRetentionPolicy => new MeasurementCommand(Sensors.LivingRoomSensor, MeasurementType.Temperature, RetentionPolicy.Hour, 23.7, Clock.Today);
            public static MeasurementCommand LivingRoomTemperatureWithDayRetentionPolicy => new MeasurementCommand(Sensors.LivingRoomSensor, MeasurementType.Temperature, RetentionPolicy.Day, 23.7, Clock.Today);
            public static MeasurementCommand LivingRoomTemperatureWithWeekRetentionPolicy => new MeasurementCommand(Sensors.LivingRoomSensor, MeasurementType.Temperature, RetentionPolicy.Week, 23.7, Clock.Today);
            public static MeasurementCommand CumulativePowerImportMeasurement1 = new MeasurementCommand(Sensors.PowerMeter, MeasurementType.CumulativePowerImport, RetentionPolicy.None, 150000, DateTime.UtcNow.AddHours(-1));
            public static MeasurementCommand CumulativePowerImportMeasurement2 = new MeasurementCommand(Sensors.PowerMeter, MeasurementType.CumulativePowerImport, RetentionPolicy.None, 150000, DateTime.UtcNow);
        }

        public static class Locations
        {
            public static CreateLocationCommand Home => new("Home", "Description of the Home location");


            public static CreateLocationCommand Cabin => new("Cabin", "Description of the Cabin location");

        }

        public static class VatRates
        {
            public static PostVATRateCommand Vat25 = new("TestVatRate", 25);
        }

        public static class EnergyPriceAreas
        {
            public static PostEnergyPriceAreaCommand Norway3 = new("10YNO-2--------T", "NO2", "Description", 1, 1);
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
                new(LocationId: 0, Name: LivingRoomName, Description: LivingRoomDescription);

            public static CreateZoneCommand Outside =>
                new(LocationId: 0, Name: OutsideName, Description: OutsideDescription);

            public static CreateZoneCommand Kitchen =>
                new(LocationId: 0, Name: KitchenName, Description: KitchenDescription);

            public static CreateZoneCommand PowerMeter =>
                new(LocationId: 0, Name: PowerMeterName, Description: PowerMeterDescription);

            public static CreateZoneCommand TestZone =>
                new(LocationId: 0, Name: TestZoneName, Description: TestZoneDescription);
        }

        public static class Heaters
        {

            public const string LivingRoomHeaterName = "LivingRoomHeater";
            public const string LivingRoomHeaterDescription = "Description of the LivingRoomHeater";
            public const string LivingRoomHeaterMqttTopic = "LivingRoomHeaterTopic";
            public const string LivingRoomHeaterOnPayload = "ON";
            public const string LivingRoomHeaterOffPayload = "OFF";

            public const string UpdatedLivingRoomHeaterName = "UpdatedLivingRoomHeater";
            public const string UpdatedLivingRoomHeaterDescription = "UpdatedDescription of the LivingRoomHeater";
            public const string UpdatedLivingRoomHeaterMqttTopic = "UpdatedLivingRoomHeaterTopic";
            public const string UpdatedLivingRoomHeaterOnPayload = "UPDATEDON";
            public const string UpdatedLivingRoomHeaterOffPayload = "UPDATEDOFF";


            public const string LivingRoomHeater2Name = "LivingRoomHeater2";
            public const string LivingRoomHeater2Description = "Description of the LivingRoomHeater2";
            public const string LivingRoomHeater2MqttTopic = "LivingRoomHeater2Topic";
            public const string LivingRoomHeater2OnPayload = "ON";
            public const string LivingRoomHeater2OffPayload = "OFF";

            public const string KitchenHeaterName = "KitchenHeater";
            public const string KitchenHeaterDescription = "Description of the KitchenHeater";
            public const string KitchenHeaterMqttTopic = "KitchenHeaterTopic";
            public const string KitchenHeaterOnPayload = "ON";
            public const string KitchenHeaterOffPayload = "OFF";

            public const string TestHeaterName = "TestHeater";
            public const string TestHeaterDescription = "Description of the TestHeater";
            public const string TestHeaterMqttTopic = "TestHeaterTopic";
            public const string TestHeaterOnPayload = "ON";
            public const string TestHeaterOffPayload = "OFF";

            public static CreateHeaterCommand LivingRoomHeater1(long livingRoomZoneId) =>
                new(Name: LivingRoomHeaterName, Description: LivingRoomHeaterDescription, MqttTopic: LivingRoomHeaterMqttTopic, OnPayload: LivingRoomHeaterOnPayload, OffPayload: LivingRoomHeaterOffPayload, ZoneId: livingRoomZoneId);

            public static CreateHeaterCommand LivingRoomHeater2(long livingRoomZoneId) =>
                new(Name: LivingRoomHeater2Name, Description: LivingRoomHeater2Description, MqttTopic: LivingRoomHeater2MqttTopic, OnPayload: LivingRoomHeater2OnPayload, OffPayload: LivingRoomHeater2OffPayload, ZoneId: livingRoomZoneId);

            public static CreateHeaterCommand KitchenHeater(long kitchenZoneId) =>
                new(Name: KitchenHeaterName, Description: KitchenHeaterDescription, MqttTopic: KitchenHeaterMqttTopic, OnPayload: KitchenHeaterOnPayload, OffPayload: KitchenHeaterOffPayload, ZoneId: kitchenZoneId);

            public static CreateHeaterCommand TestHeater(long testZoneId) =>
                new(Name: TestHeaterName, Description: TestHeaterDescription, MqttTopic: TestHeaterMqttTopic, OnPayload: TestHeaterOnPayload, OffPayload: TestHeaterOffPayload, ZoneId: testZoneId);

            public static UpdateHeaterCommand UpdateHeater(long heaterId) =>
                new(heaterId, UpdatedLivingRoomHeaterName, UpdatedLivingRoomHeaterDescription, UpdatedLivingRoomHeaterMqttTopic, UpdatedLivingRoomHeaterOnPayload, UpdatedLivingRoomHeaterOffPayload);
        }


        public static class Users
        {
            public static CreateUserCommand StandardUser =>
                new(Email: "StandardUser@tempuri.org", FirstName: "FirstName", LastName: "LastName", IsAdmin: false, NewPassword: ValidPassword, ConfirmedPassword: ValidPassword);
            public static CreateUserCommand AnotherStandardUser =>
                new(Email: "AnotherStandardUser@tempuri.org", FirstName: "FirstName", LastName: "LastName", IsAdmin: false, NewPassword: ValidPassword, ConfirmedPassword: ValidPassword);

            public static CreateUserCommand StandardUserWithWeakPassword =>
                new(Email: "StandardUser@tempuri.org", FirstName: "FirstName", LastName: "LastName", IsAdmin: false, NewPassword: "abc123", ConfirmedPassword: "abc123");

            public static CreateUserCommand StandardUserWithGivenPassword(string password) =>
                new(Email: "StandardUser@tempuri.org", FirstName: "FirstName", LastName: "LastName", IsAdmin: false, NewPassword: password, ConfirmedPassword: password);

            public static CreateUserCommand StandardUserWithInvalidEmail =>
                new(Email: "InvalidMailAddress", FirstName: "FirstName", LastName: "LastName", IsAdmin: false, NewPassword: ValidPassword, ConfirmedPassword: ValidPassword);
        }
        public static class Sensors
        {
            public static string LivingRoomSensor = "SensorID1";

            public static string OutsideSensor = "SensorID2";

            public static string PowerMeter = "PM1234";

            public static string KitchenSensor = "SensorID3";
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

            public static UpdateScheduleCommand ScheduleWithInvalidCronExpression(long scheduleId) =>
                new(scheduleId, TestScheduleUpdatedName, InvalidCronExpression);
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


        public static class Mqtt
        {
            public const string OnPayload = "ON";
            public const string OffPayload = "OFF";

            public const string TestTopic = "TestTopic";

            public static PublishMqttMessageCommand TestPublishMqttMessageCommand(string payload) =>
                new(TestTopic, payload);
        }

        public static class PushSubscriptions
        {
            public static string Endpoint = "https://example.com";

            public static string P256dh = "BOrw9";

            public static string Auth = "BOrw9";

            public static CreatePushSubscriptionCommand TestPushSubscription = new(Endpoint, new(P256dh, Auth));


        }

        public static class Clock
        {
            public static DateTime Today => new(1972, 1, 21, 14, 15, 36, DateTimeKind.Utc);

            public static DateTime LaterToday => new(1972, 1, 21, 14, 15, 37, DateTimeKind.Utc);
        }


    }


}
