using System;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using DbReader;
using HeatKeeper.Abstractions.Logging;
using HeatKeeper.Server.Database.Migrations;

namespace HeatKeeper.Server.Database
{
    public interface IDatabaseMigrator
    {
        void Migrate();
    }

    public class DatabaseMigrator : IDatabaseMigrator
    {
        private readonly ApplicationConfiguration configuration;
        private readonly ISqlProvider sqlProvider;
        private readonly Logger log;

        public DatabaseMigrator(ApplicationConfiguration configuration, ISqlProvider sqlProvider, Logger log)
        {
            this.configuration = configuration;
            this.sqlProvider = sqlProvider;
            this.log = log;
        }

        public void Migrate()
        {
            using (var connection = new SQLiteConnection(configuration.ConnectionString))
            {
                log.Debug($"Open database connection using connection string: {configuration.ConnectionString}");
                connection.Open();
                var appliedVersions = GetDatabaseVersion(connection);
                var databaseVersion = appliedVersions.OrderBy(vi => vi.Version).LastOrDefault()?.Version ?? 0;
                log.Info($"Database is at version {databaseVersion}");

                var migrations = typeof(DatabaseMigrator).Assembly.GetTypes()
                    .Where(t => typeof(IMigration).IsAssignableFrom(t) && t.IsClass)
                    .Select(t => new { MigrationType = t, ((AppliesToVersionAttribute)t.GetCustomAttributes(typeof(AppliesToVersionAttribute), true).Single()).Version })
                    .Where(m => m.Version > databaseVersion).OrderBy(m => m.Version).ToArray();

                foreach (var migration in migrations)
                {
                    log.Info($"Applying migration {migration.MigrationType.Name}");
                    var migrationInstance = (IMigration)Activator.CreateInstance(migration.MigrationType, sqlProvider);
                    migrationInstance.Migrate(connection);
                    connection.Execute(sqlProvider.InsertVersionInfo, new VersionInfo() { Version = migration.Version, AppliedOn = DateTime.UtcNow, Description = migration.MigrationType.Name });
                    log.Info($"Database is now at version {migration.Version}");
                }
                connection.Close();
            }
        }

        private VersionInfo[] GetDatabaseVersion(IDbConnection connection)
        {
            var isEmpty = connection.ExecuteScalar<long>(sqlProvider.IsEmptyDatabase) == 1;
            if (isEmpty)
            {
                return Array.Empty<VersionInfo>();
            }
            else
            {
                return connection.Read<VersionInfo>(sqlProvider.GetVersionInfo).ToArray();
            }
        }
    }
}
