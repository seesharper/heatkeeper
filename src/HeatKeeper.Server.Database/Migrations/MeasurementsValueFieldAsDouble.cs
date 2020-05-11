using System.Data;
using DbReader;

namespace HeatKeeper.Server.Database.Migrations
{
    [AppliesToVersion(3)]
    public class MeasurementsValueFieldAsDouble : IMigration
    {
        private readonly ISqlProvider sqlProvider;

        public MeasurementsValueFieldAsDouble(ISqlProvider sqlProvider)
        {
            this.sqlProvider = sqlProvider;
        }

        public void Migrate(IDbConnection dbConnection)
            => dbConnection.Execute(sqlProvider.MeasurementsValueFieldAsDouble);
    }
}
