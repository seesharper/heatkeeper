using System.Data;
using DbReader;

namespace HeatKeeper.Server.Database.Migrations
{
    [AppliesToVersion(4)]
    public class AddRetentionPolicyColumn : IMigration
    {
        private readonly ISqlProvider sqlProvider;

        public AddRetentionPolicyColumn(ISqlProvider sqlProvider)
            => this.sqlProvider = sqlProvider;

        public void Migrate(IDbConnection dbConnection)
            => dbConnection.Execute(sqlProvider.AddRetentionPolicyColumn);
    }
}
