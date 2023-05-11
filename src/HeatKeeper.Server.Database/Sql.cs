namespace HeatKeeper.Server.Database
{
    public interface ISqlProvider
    {
        string CreateDatabase { get; }

        string IsEmptyDatabase { get; }

        string InsertVersionInfo { get; }

        string InsertZone { get; }

        string GetAllZones { get; }

        string GetZoneId { get; }

        string ClearZoneFromAllSensors { get; }

        string ClearZoneFromAllLocations { get; }

        string DeleteZone { get; }

        string UpdateZone { get; }

        string GetZoneDetails { get; }

        string InsertLocation { get; }

        string UpdateLocation { get; }

        string GetAllLocations { get; }

        string DeleteLocation { get; }

        string DeleteAllUsersFromLocation { get; }

        string GetLocationId { get; }

        string InsertMeasurement { get; }

        string InsertSensor { get; }

        string GetAllSensors { get; }

        string AddSensorToZone { get; }

        string RemoveSensorFromZone { get; }

        string GetSensorsByZone { get; }

        string GetAllExternalSensors { get; }

        string InsertUser { get; }

        string InsertAdminUser { get; }

        string GetUserId { get; }

        string GetUser { get; }

        string DeleteUser { get; }

        string DeleteUserLocations { get; }

        string InsertUserLocation { get; }

        string UpdatePasswordHash { get; }

        string GetUserLocationId { get; }

        string GetAllUsers { get; }

        string UpdateUser { get; }

        string UpdateCurrentUser { get; }

        string UserExists { get; }

        string GetUsersByLocation { get; }

        string LocationExists { get; }

        string ZonesByLocation { get; }

        string ZoneExists { get; }

        string LocationUserExists { get; }

        string UpdateDefaultInsideZone { get; }

        string UpdateDefaultOutsideZone { get; }

        string ClearDefaultInsideZone { get; }

        string ClearDefaultOutsideZone { get; }

        string GetZoneIdByExternalSensorId { get; }

        string LatestZoneMeasurementExists { get; }

        string InsertLatestZoneMeasurement { get; }

        string UpdateLatestZoneMeasurement { get; }

        string GetVersionInfo { get; }

        string CreateLatestZoneMeasurementsTable { get; }

        string GetAllDashboardLocations { get; }

        string DeleteSensor { get; }

        string UpdateSensor { get; }

        string MeasurementsToSensonsForeignKey { get; }

        string DeleteSensorMeasurements { get; }

        string GetMeasurementsToExport { get; }

        string DeleteExportedMeasurements { get; }

        string UpdateExportedMeasurement { get; }

        string MeasurementsValueFieldAsDouble { get; }

        string LatestMeasurements { get; }

        string AddRetentionPolicyColumn { get; }

        string CreateProgramsTable { get; }

        string CreateSchedulesTable { get; }

        string CreateSetPointsTable { get; }

        string CreateForeignKeyFromProgramsToSchedules { get; }

        string CreateForeignKeyFromLocationsToPrograms { get; }

        string AddMqttTopicToZonesTable { get; }

        string InsertProgram { get; }

        string InsertSchedule { get; }

        string InsertSetPoint { get; }

        string UpdateSetPoint { get; }

        string UpdateSchedule { get; }

        string UpdateProgram { get; }

        string DeleteSchedule { get; }

        string DeleteSetPoint { get; }

        string DeleteProgram { get; }

        string SetActiveScheduleIdToNull { get; }

        string SetActiveSchedule { get; }

        string GetLastInsertedRowId { get; }

        string GetProgramsByLocation { get; }

        string GetSchedulesByProgram { get; }

        string GetAllSchedules { get; }

        string GetSetPointsBySchedule { get; }

        string ActivateProgram { get; }

        string ClearActiveProgram { get; }

        string GetTargetSetPoints { get; }

        string GetMeasuredTemperatureValuePerZone { get; }

        string GetZoneMqttInfo { get; }

        string AddLastSeenColumnToSensors { get; }

        string UpdateLastSeenOnSensor { get; }

        string GetDeadSensors { get; }

        string GetZoneAndLocationByZoneId { get; }
    }
}
