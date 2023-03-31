using System.Data;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using DbReader;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;

namespace HeatKeeper.Server.Measurements
{
    public class UpdateLatestMeasurementCommandHandler : ICommandHandler<UpdateLatestMeasurementCommand>
    {
        private readonly IDbConnection dbConnection;
        private readonly ISqlProvider sqlProvider;

        public UpdateLatestMeasurementCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            this.dbConnection = dbConnection;
            this.sqlProvider = sqlProvider;
        }

        public async Task HandleAsync(UpdateLatestMeasurementCommand command, CancellationToken cancellationToken = default)
        {
            await dbConnection.ExecuteAsync(sqlProvider.UpdateLatestZoneMeasurement, command);
        }
    }


    [RequireReporterRole]
    public class UpdateLatestMeasurementCommand : LatestZoneMeasurementCommand
    {
    }
}
