using System;
using System.IO;

namespace HeatKeeper.Server.Database
{
    public class SqliteConnectionStringProvider : IConnectionStringProvider
    {
        public string GetConnectionString()
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var pathToDatabaseFolder = Path.Combine(baseDirectory, "..", "db");
            if (!Directory.Exists(pathToDatabaseFolder))
            {
                Directory.CreateDirectory(pathToDatabaseFolder);
            }

            var pathToDatabaseFile = Path.Combine(Path.GetFullPath(pathToDatabaseFolder), "heatkeeper.db");
            return $"data source = {pathToDatabaseFile}";
        }
    }
}