using System.Threading.Tasks;
using System.Threading;
using System.Data.Common;
using System.Data;
using HeatKeeper.Server.Database;
using DbReader;
using CQRS.Command.Abstractions;

namespace HeatKeeper.Server.Measurements
{
    public class CreateMeasurementsCommandHandler : ICommandHandler<CreateMeasurementCommand[]>
    {
        private readonly IDbConnection dbConnection;
        private readonly ISqlProvider sqlProvider;

        public CreateMeasurementsCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            this.dbConnection = dbConnection;
            this.sqlProvider = sqlProvider;
        }

        public async Task HandleAsync(CreateMeasurementCommand[] commands, CancellationToken cancellationToken = default)
        {
            foreach (var command in commands)
            {
                await ((DbCommand)dbConnection.CreateCommand(sqlProvider.InsertMeasurement, command)).ExecuteNonQueryAsync();
            }
        }
    }
}
