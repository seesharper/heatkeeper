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
    public class UpdateExportedMeasurementsCommandHandler : ICommandHandler<UpdateExportedMeasurementsCommand[]>
    {
        private readonly IDbConnection dbConnection;
        private readonly ISqlProvider sqlProvider;

        public UpdateExportedMeasurementsCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            this.dbConnection = dbConnection;
            this.sqlProvider = sqlProvider;
        }

        public async Task HandleAsync(UpdateExportedMeasurementsCommand[] commands, CancellationToken cancellationToken = default)
        {
            foreach (var command in commands)
            {
                await dbConnection.ExecuteAsync(sqlProvider.UpdateExportedMeasurement, command);
            }
        }
    }

    [RequireReporterRole]
    public struct UpdateExportedMeasurementsCommand
    {
        public long MeasurementId { get; set; }

        public DateTime Exported { get; set; }
    }
}
