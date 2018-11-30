using System.IO;
using System.Text;

namespace HeatKeeper.Server.Database
{
    public interface ISqlProvider
    {
        string InsertZone {get;}
    }

    public class SqlProvider : ISqlProvider
    {


        public string InsertZone {get => Load("Zones.InsertZone"); }

        public string Load(string name)
        {
            throw new System.NotImplementedException();
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

    public enum QueryName
    {

    }
}