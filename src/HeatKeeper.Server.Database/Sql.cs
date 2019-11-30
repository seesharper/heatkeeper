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
        string InsertZone { get; }

        string GetAllZones { get; }

        string InsertLocation { get; }

        string GetAllLocations { get; }

        string GetLocationId { get; }

        string InsertMeasurement { get; }

        string InsertSensor { get; }

        string GetAllSensors { get; }

        string GetAllExternalSensors { get; }

        string InsertUser { get; }

        string GetUserId { get; }

        string GetUser { get; }

        string InsertUserLocation { get; }

        string UpdatePasswordHash { get; }

        string DeleteUserLocation { get; }

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
    }
}
