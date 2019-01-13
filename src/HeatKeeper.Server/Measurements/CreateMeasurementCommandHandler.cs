using System.Threading.Tasks;
using System.Threading;
using System.Data.Common;
using System.Data;
using HeatKeeper.Server.Database;
using HeatKeeper.Server.CQRS;
using DbReader;

namespace HeatKeeper.Server.Measurements
{
    public class CreateMeasurementCommandHandler : ICommandHandler<CreateMeasurementCommand>
    {
        private readonly IDbConnection dbConnection;
        private readonly ISqlProvider sqlProvider;

        public CreateMeasurementCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            this.dbConnection = dbConnection;
            this.sqlProvider = sqlProvider;
        }

        public async Task HandleAsync(CreateMeasurementCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            await ((DbCommand)dbConnection.CreateCommand(sqlProvider.InsertMeasurement, command)).ExecuteNonQueryAsync();
        }
    }
}