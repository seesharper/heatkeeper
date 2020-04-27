using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using CQRS.Query.Abstractions;

namespace HeatKeeper.Server.Measurements
{
    public class MaintainLatestZoneMeasurementDecorator : ICommandHandler<MeasurementCommand>
    {
        private readonly ICommandHandler<MeasurementCommand> handler;
        private readonly ICommandExecutor commandExecutor;
        private readonly IQueryExecutor queryExecutor;

        public MaintainLatestZoneMeasurementDecorator(ICommandHandler<MeasurementCommand> handler, ICommandExecutor commandExecutor, IQueryExecutor queryExecutor)
        {
            this.handler = handler;
            this.commandExecutor = commandExecutor;
            this.queryExecutor = queryExecutor;
        }

        public async Task HandleAsync(MeasurementCommand command, CancellationToken cancellationToken = default)
        {
            await handler.HandleAsync(command);
            var zoneId = await queryExecutor.ExecuteAsync(new ZoneByExternalSensorQuery() { ExternalSensorId = command.SensorId });
            if (zoneId == null)
            {
                return;
            }

            var latestZoneMeasurementExists = await queryExecutor.ExecuteAsync(new LatestZoneMeasurementExistsQuery() { ZoneId = zoneId.Value, MeasurementType = command.MeasurementType });
            if (latestZoneMeasurementExists)
            {
                await commandExecutor.ExecuteAsync(new UpdateLatestMeasurementCommand() { MeasurementType = command.MeasurementType, ZoneId = zoneId.Value, Value = command.Value, Updated = command.Created });
            }
            else
            {
                await commandExecutor.ExecuteAsync(new InsertLatestZoneMeasurementCommand() { MeasurementType = command.MeasurementType, ZoneId = zoneId.Value, Value = command.Value, Updated = command.Created });
            }
        }
    }


}
