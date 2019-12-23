using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace HeatKeeper.Server.Database
{
    public interface ISqlProvider
    {
        string CreateDatabase { get; }

        string InsertZone { get; }

        string GetAllZones { get; }

        string GetZoneId { get; }

        string GetZoneDetails { get; }

        string InsertLocation { get; }

        string GetAllLocations { get; }

        string GetLocationId { get; }

        string InsertMeasurement { get; }

        string InsertSensor { get; }

        string GetAllSensors { get; }

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
    }
}
