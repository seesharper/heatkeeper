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

        string GetUser {get;}

        string InsertUserLocation {get;}

        string UpdatePasswordHash {get;}

        string DeleteUserLocation {get;}

        string GetUserLocationId { get; }

        string GetAllUsers {get;}

        string UpdateUser {get;}

        string UserExists {get;}

        string LocationExists{get;}

        string ZonesByLocation {get;}

        string ZoneExists {get;}
    }

    public class SqlProvider : ISqlProvider
    {
        private static ConcurrentDictionary<string,string> sqlCache = new ConcurrentDictionary<string, string>();

        public string InsertZone => Load();

        public string GetAllZones => Load();

        public string InsertLocation=> Load();

        public string GetAllLocations => Load();

        public string InsertMeasurement => Load();

        public string InsertSensor => Load();

        public string GetAllSensors => Load();

        public string GetAllExternalSensors => Load();

        public string InsertUser => Load();
        public string GetUser=> Load();

        public string InsertUserLocation => Load();

        public string GetLocationId => Load();

        public string GetUserId => Load();

        public string UpdatePasswordHash => Load();

        public string DeleteUserLocation => Load();

        public string GetUserLocationId => Load();

        public string GetAllUsers => Load();

        public string UpdateUser => Load();

        public string UserExists => Load();

        public string LocationExists => Load();

        public string ZonesByLocation => Load();

        public string ZoneExists => Load();

        public string Load([CallerMemberName] string name = "")
        {
            return LoadSql(name);
        }

        private static string LoadSql(string name)
        {
            return sqlCache.GetOrAdd(name, FindSqlResource);
        }

        private static string FindSqlResource(string name)
        {
            var assembly = typeof(SqlProvider).Assembly;
            var resourceNames = assembly.GetManifestResourceNames();
            var sqlResource = resourceNames.FirstOrDefault(r => r.EndsWith($"{name}.sql",StringComparison.InvariantCultureIgnoreCase));
            if (sqlResource == null)
            {

            }
            var resourceStream = assembly.GetManifestResourceStream(sqlResource);
            using (var reader = new StreamReader(resourceStream, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }
    }
}