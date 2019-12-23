using System;
using System.Data.SQLite;
using DbReader;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;

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

        public DatabaseMigrator(ApplicationConfiguration configuration, ISqlProvider sqlProvider)
        {
            this.configuration = configuration;
            this.sqlProvider = sqlProvider;
        }

        public void Migrate()
        {
            using (var connection = new SQLiteConnection(configuration.ConnectionString))
            {
                connection.Open();
                connection.Execute(sqlProvider.CreateDatabase);
                connection.Execute(sqlProvider.InsertAdminUser, new { Email = AdminUser.DefaultEmail, Firstname = AdminUser.DefaultFirstName, Lastname = AdminUser.DefaultLastName, HashedPassword = AdminUser.DefaultPasswordHash });
            }

            // var serviceProvider = CreateServices(configuration.ConnectionString);

            // // Put the database update into a scope to ensure
            // // that all resources will be disposed.
            // using (var scope = serviceProvider.CreateScope())
            // {
            //     UpdateDatabase(scope.ServiceProvider);
            // }
        }

        private static IServiceProvider CreateServices(string connectionString)
        {
            return new ServiceCollection()
                // Add common FluentMigrator services
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    // Add SQLite support to FluentMigrator
                    .AddSQLite()
                    // Set the connection string
                    .WithGlobalConnectionString(connectionString)
                    // Define the assembly containing the migrations
                    .ScanIn(typeof(DatabaseMigrator).Assembly).For.Migrations())
                // Enable logging to console in the FluentMigrator way
                .AddLogging(lb => lb.AddFluentMigratorConsole())
                // Build the service provider
                .BuildServiceProvider(false);
        }


        private static void UpdateDatabase(IServiceProvider serviceProvider)
        {
            // Instantiate the runner
            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

            // Execute the migrations
            runner.MigrateUp();
        }
    }
}
