namespace HeatKeeper.Server.Database;

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

    string GetUnassignedSensors { get; }

    string AssignZoneToSensor { get; }

    string RemoveZoneFromSensor { get; }

    string GetSensorsByZone { get; }

    string GetAllExternalSensors { get; }

    string InsertUser { get; }

    string InsertAdminUser { get; }

    string GetUserId { get; }

    string GetUser { get; }

    string DeleteUser { get; }

    string DeleteUserLocations { get; }

    string DeleteUserLocation { get; }

    string InsertUserLocation { get; }

    string UpdatePasswordHash { get; }

    string GetUserLocationId { get; }

    string GetAllUsers { get; }

    string UpdateUser { get; }

    string UpdateCurrentUser { get; }

    string UserExists { get; }

    string GetUsersByLocation { get; }

    string GetUserLocationsAccess { get; }

    string GetUserDetails { get; }

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

    string MeasurementsValueFieldAsDouble { get; }

    string LatestMeasurements { get; }

    string GetMeasurements { get; }

    string DeleteExpiredMeasurements { get; }

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

    string DeleteAllSetPoints { get; }

    string UpdateSchedule { get; }

    string UpdateProgram { get; }

    string DeleteSchedule { get; }

    string DeleteAllSchedules { get; }

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

    string CreateRefreshTokensTable { get; }

    string InsertRefreshToken { get; }

    string DeleteRefreshToken { get; }

    string UpdateRefreshToken { get; }

    string GetDashboardTemperatures { get; }

    string GetLocationDetails { get; }

    string GetProgramDetails { get; }

    string AddDescriptionToProgramsTable { get; }

    string GetScheduleDetails { get; }

    string GetSetPointDetails { get; }

    string GetZonesNotAssignedToSchedule { get; }

    string CreateHeatersTable { get; }

    string GetHeaters { get; }

    string InsertHeater { get; }

    string DeleteHeater { get; }

    string UpdateHeater { get; }

    string GetHeaterDetails { get; }

    string GetHeatersMqttInfo { get; }

    string GetSensorDetails { get; }

    string DeleteOldPushSubscriptions { get; }

    string InsertPushSubscription { get; }

    string PushSubscriptionExists { get; }

    string UpdateLastSeenOnPushSubscription { get; }

    string GetPushSubscriptionsByLocation { get; }

    string CreatePushSubscriptionsTable { get; }

    string NewLocationExists { get; }

    string CreateVATRatesTable { get; }

    string CreateEnergyPricesTable { get; }

    string CreateEnergyPriceAreasTable { get; }

    string InsertVATRate { get; }

    string GetVATRateDetails { get; }

    string UpdateVATRate { get; }

    string DeleteVATRate { get; }

    string GetVATRates { get; }

    string InsertEnergyPriceArea { get; }

    string GetEnergyPriceAreaDetails { get; }

    string UpdateEnergyPriceArea { get; }

    string DeleteEnergyPriceArea { get; }

    string GetEnergyPriceAreas { get; }

    string InsertEnergyPrice { get; }

    string GetConfiguredPriceAreas { get; }

    string GetEnergyPrices { get; }

    string EnergyPricesExists { get; }

    string AddMinutesBeforeConsideredDeadToSensorsTable { get; }

    string InsertNotificationSubscription { get; }



    string UpdateNotification { get; }

    string DeleteNotification { get; }

    string DeleteAllNotificationSubscriptions { get; }

    string DeleteAllNotificationConditions { get; }

    string DeleteNotificationSubscription { get; }

    string CreateNotificationsTable { get; }

    string GetNotificationSubscriptions { get; }

    string GetNotificationDetails { get; }

    string CreateNotificationConditionsTable { get; }

    string GetUsersSubscribedToNotification { get; }

    string GetPushSubscriptionByUser { get; }

    string GetAllScheduledNotifications { get; }

    string GetNotificationSendingDetails { get; }

    string CreateNotificationSubscriptionsTable { get; }
    string InsertNotification { get; }

    string GetNotifications { get; }

    string AddLongitudeAndLatitudeToLocationsTable { get; }

    string GetLocationCoordinates { get; }

    string CreateEventTriggersTable { get; }

    string GetAllEventTriggers { get; }

    string InsertEventTrigger { get; }

    string UpdateEventTrigger { get; }

    string DeleteEventTrigger { get; }

}
