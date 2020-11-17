using System.Data;
using DbReader;

namespace HeatKeeper.Server.Database.Migrations
{
    [AppliesToVersion(1)]
    public class CreateDatabaseMigration : IMigration
    {
        private readonly ISqlProvider sqlProvider;

        public CreateDatabaseMigration(ISqlProvider sqlProvider)
            => this.sqlProvider = sqlProvider;

        public void Migrate(IDbConnection dbConnection)
        {
            dbConnection.Execute(sqlProvider.CreateDatabase);
            dbConnection.Execute(sqlProvider.InsertAdminUser, new { Email = AdminUser.DefaultEmail, Firstname = AdminUser.DefaultFirstName, Lastname = AdminUser.DefaultLastName, HashedPassword = AdminUser.DefaultPasswordHash });
        }
    }
}
