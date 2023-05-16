using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using DbReader;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;

namespace HeatKeeper.Server.Measurements
{
    public class InsertLatestZoneMeasurementCommandHandler : ICommandHandler<InsertLatestZoneMeasurementCommand>
    {
        private readonly IDbConnection _dbConnection;
        private readonly ISqlProvider _sqlProvider;

        public InsertLatestZoneMeasurementCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            _dbConnection = dbConnection;
            _sqlProvider = sqlProvider;
        }

        public async Task HandleAsync(InsertLatestZoneMeasurementCommand command, CancellationToken cancellationToken = default)
            => await _dbConnection.ExecuteAsync(_sqlProvider.InsertLatestZoneMeasurement, command);
    }

    [RequireReporterRole]
    public class InsertLatestZoneMeasurementCommand : LatestZoneMeasurementCommand
    {
    }
}
