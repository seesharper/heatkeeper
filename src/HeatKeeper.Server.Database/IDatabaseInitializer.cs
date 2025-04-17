using System;
using System.Data;
using System.Linq;
using DbReader;
using HeatKeeper.Abstractions.Configuration;
using HeatKeeper.Server.Database.Migrations;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HeatKeeper.Server.Database
{
    public interface IDatabaseMigrator
    {
        void Migrate();
    }

    public class DatabaseMigrator : IDatabaseMigrator
    {
        private readonly ISqlProvider _sqlProvider;
        private readonly ILogger<DatabaseMigrator> _logger;

        private readonly IConfiguration _configuration;

        public DatabaseMigrator(IConfiguration configuration, ISqlProvider sqlProvider, ILogger<DatabaseMigrator> logger)
        {
            _configuration = configuration;
            _sqlProvider = sqlProvider;
            _logger = logger;
        }

        public void Migrate()
        {
            using (var connection = new SqliteConnection(_configuration.GetConnectionString()))
            {
                _logger.LogDebug("Open database connection using connection string: {_configuration.ConnectionString}", _configuration.GetConnectionString());
                connection.Open();
                using var transaction = connection.BeginTransaction();
                try
                {
                    var appliedVersions = GetDatabaseVersion(connection);
                    var databaseVersion = appliedVersions.OrderBy(vi => vi.Version).LastOrDefault()?.Version ?? 0;
                    _logger.LogInformation("Database is at version {databaseVersion}", databaseVersion);

                    var migrationsGroupedByVersion = GetMigrationsGroupedByVersion(databaseVersion);

                    foreach (var migrations in migrationsGroupedByVersion)
                    {
                        var appliedMigrationsDescription = string.Empty;
                        _logger.LogInformation("Applying {migrations.Count()} migration(s) to be applied for version {migrations.Key}", migrations.Count(), migrations.Key);
                        foreach (var migration in migrations.OrderBy(m => m.Order))
                        {
                            _logger.LogInformation("Applying migration {migration.MigrationType.Name}", migration.MigrationType.Name);
                            var migrationInstance = (IMigration)Activator.CreateInstance(migration.MigrationType, _sqlProvider);
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
                        connection.Execute(_sqlProvider.InsertVersionInfo, new VersionInfo() { Version = migrations.Key, AppliedOn = DateTime.UtcNow, Description = appliedMigrationsDescription });
                        _logger.LogInformation("Database is now at version {migrations.Key}", migrations.Key);

                    }
                }
                catch (System.Exception)
                {
                    transaction.Rollback();
                    throw;
                }
                transaction.Commit();
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
            var isEmpty = connection.ExecuteScalar<long>(_sqlProvider.IsEmptyDatabase) == 1;
            if (isEmpty)
            {
                return Array.Empty<VersionInfo>();
            }
            else
            {
                return connection.Read<VersionInfo>(_sqlProvider.GetVersionInfo).ToArray();
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
