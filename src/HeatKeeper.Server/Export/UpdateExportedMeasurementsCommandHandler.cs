using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using DbReader;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;

namespace HeatKeeper.Server.Export
{
    public class UpdateExportedMeasurementsCommandHandler : ICommandHandler<UpdateExportedMeasurementsCommand>
    {
        private readonly IDbConnection _dbConnection;
        private readonly ISqlProvider _sqlProvider;

        public UpdateExportedMeasurementsCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            _dbConnection = dbConnection;
            _sqlProvider = sqlProvider;
        }

        public async Task HandleAsync(UpdateExportedMeasurementsCommand command, CancellationToken cancellationToken = default)
        {
            foreach (var exportedMeasurement in command.ExportedMeasurements)
            {
                await _dbConnection.ExecuteAsync(_sqlProvider.UpdateExportedMeasurement, exportedMeasurement);
            }
        }
    }

    [RequireBackgroundRole]
    public record UpdateExportedMeasurementsCommand(ExportedMeasurement[] ExportedMeasurements);

    public record ExportedMeasurement(long MeasurementId, DateTime Exported);
}
