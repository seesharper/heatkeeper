using System.IO;
using System.Text;

namespace HeatKeeper.Server.Database
{
    public interface ISqlProvider
    {
        string InsertZone { get; }

        string GetAllZones { get; }
    }

    public class SqlProvider : ISqlProvider
    {

        public string InsertZone { get => Load("Zones.InsertZone"); }

        public string GetAllZones { get => Load("Zones.GetAllZones"); }

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