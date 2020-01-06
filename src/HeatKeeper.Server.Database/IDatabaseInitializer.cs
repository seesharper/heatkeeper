using System;
using System.Data.SQLite;
using DbReader;

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
                var isEmpty = connection.ExecuteScalar<long>(sqlProvider.IsEmptyDatabase) == 1 ? true : false;
                if (isEmpty)
                {
                    connection.Execute(sqlProvider.CreateDatabase);
                    connection.Execute(sqlProvider.InsertAdminUser, new { Email = AdminUser.DefaultEmail, Firstname = AdminUser.DefaultFirstName, Lastname = AdminUser.DefaultLastName, HashedPassword = AdminUser.DefaultPasswordHash });
                    connection.Execute(sqlProvider.InsertVersionInfo, new VersionInfo() { Version = 1, AppliedOn = DateTime.UtcNow, Description = "Initial version" });
                }
            }
        }
    }
}
