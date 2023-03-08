using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using CQRS.Query.Abstractions;
using HeatKeeper.Abstractions.Logging;
using HeatKeeper.Server.Export;

namespace HeatKeeper.Server.Measurements
{
    public class ExportMeasurementsDecorator : ICommandHandler<MeasurementCommand[]>
    {
        private readonly ICommandHandler<MeasurementCommand[]> handler;
        private readonly IQueryExecutor queryExecutor;
        private readonly ICommandExecutor commandExecutor;
        private readonly Logger logger;

        public ExportMeasurementsDecorator(ICommandHandler<MeasurementCommand[]> handler, IQueryExecutor queryExecutor, ICommandExecutor commandExecutor, Logger logger)
        {
            this.handler = handler;
            this.queryExecutor = queryExecutor;
            this.commandExecutor = commandExecutor;
            this.logger = logger;
        }

        public async Task HandleAsync(MeasurementCommand[] command, CancellationToken cancellationToken = default)
        {
            await handler.HandleAsync(command);
            var measurementsToExport = await queryExecutor.ExecuteAsync(new MeasurementsToExportQuery());
            try
            {
                await commandExecutor.ExecuteAsync(new ExportMeasurementsCommand() { MeasurementsToExport = measurementsToExport });
                await commandExecutor.ExecuteAsync(measurementsToExport.Select(m => new UpdateExportedMeasurementsCommand() { MeasurementId = m.Id, Exported = DateTime.UtcNow }).ToArray());
                await commandExecutor.ExecuteAsync(new DeleteExportedMeasurementsCommand() { RetentionDate = DateTime.UtcNow.Subtract(TimeSpan.FromDays(1)) });
            }
            catch (Exception exception)
            {
                logger.Warning("Failed to export measurements", exception);
            }
        }
    }
}
