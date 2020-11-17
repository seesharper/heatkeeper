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

                var migrationsGroupedByVersion = GetMigrationsGroupedByVersion(databaseVersion);

                foreach (var migrations in migrationsGroupedByVersion)
                {
                    var appliedMigrationsDescription = string.Empty;
                    log.Info($"Found {migrations.Count()} migration(s) to be applied for version {migrations.Key}");
                    foreach (var migration in migrations.OrderBy(m => m.Order))
                    {
                        log.Info($"Applying migration {migration.MigrationType.Name}");
                        var migrationInstance = (IMigration)Activator.CreateInstance(migration.MigrationType, sqlProvider);
                        migrationInstance.Migrate(connection);
                        if (string.IsNullOrWhiteSpace(appliedMigrationsDescription))
                        {
                            appliedMigrationsDescription = migration.MigrationType.Name;
                        }
                        else
                        {
                            appliedMigrationsDescription = $"{appliedMigrationsDescription}, {migration.MigrationType.Name}";
                        }
                    }
                    connection.Execute(sqlProvider.InsertVersionInfo, new VersionInfo() { Version = migrations.Key, AppliedOn = DateTime.UtcNow, Description = appliedMigrationsDescription });
                    log.Info($"Database is now at version {migrations.Key}");
                }

                connection.Close();
            }
        }

        private static IGrouping<int, MigrationInfo>[] GetMigrationsGroupedByVersion(long databaseVersion)
        {
            return typeof(DatabaseMigrator).Assembly.GetTypes()
                .Where(t => typeof(IMigration).IsAssignableFrom(t) && t.IsClass)
                .Select(t => new MigrationInfo(t,
                    ((AppliesToVersionAttribute)t.GetCustomAttributes(typeof(AppliesToVersionAttribute), true).Single()).Version,
                    ((AppliesToVersionAttribute)t.GetCustomAttributes(typeof(AppliesToVersionAttribute), true).Single()).Order)
                )
                .GroupBy(m => m.AppliesToVersion)
                .Where(g => g.Key > databaseVersion).OrderBy(g => g.Key).ToArray();
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

        public class MigrationInfo
        {
            public MigrationInfo(Type migrationType, int appliesToVersion, int order)
            {
                MigrationType = migrationType;
                AppliesToVersion = appliesToVersion;
                Order = order;
            }

            public Type MigrationType { get; }

            public int AppliesToVersion { get; }

            public int Order { get; }
        }
    }
}
