using System.Data;
using DbReader;

namespace HeatKeeper.Server.Database.Migrations
{
    [AppliesToVersion(2)]
    public class MeasurementsToSensorsForeignKey : IMigration
    {
        private readonly ISqlProvider sqlProvider;

        public MeasurementsToSensorsForeignKey(ISqlProvider sqlProvider)
            => this.sqlProvider = sqlProvider;

        public void Migrate(IDbConnection dbConnection)
            => dbConnection.Execute(sqlProvider.MeasurementsToSensonsForeignKey);
    }
}
