using System.IO;
using System.Text;

namespace HeatKeeper.Server.Database
{
    public interface ISqlProvider
    {
        string InsertZone { get; }

        string GetAllZones { get; }

        string InsertLocation { get; }

        string GetAllLocations { get; }

        string InsertMeasurement { get; }

        string InsertSensor { get; }

        string GetAllSensors { get; }
    }

    public class SqlProvider : ISqlProvider
    {

        public string InsertZone { get => Load("Zones.InsertZone"); }

        public string GetAllZones { get => Load("Zones.GetAllZones"); }

        public string InsertLocation { get => Load("Locations.InsertLocation"); }

        public string GetAllLocations { get => Load("Locations.GetAllLocations"); }

        public string InsertMeasurement { get => Load("Measurements.InsertMeasurement"); }

        public string InsertSensor { get => Load("Sensors.InsertSensor"); }

        public string GetAllSensors { get => Load("Sensors.GetAllSensors"); }

        public string Load(string name)
        {
            return LoadSql(name);
        }

        private static string LoadSql(string name)
        {
            var assembly = typeof(SqlProvider).Assembly;
            var test = assembly.GetManifestResourceNames();
            var resourceStream = assembly.GetManifestResourceStream($"HeatKeeper.Server.Database.{name}.sql");
            using (var reader = new StreamReader(resourceStream, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }
    }
}