using System.Collections.Generic;


namespace HeatKeeper.Server.Sensors
{
    public class CreateMissingSensorsCommandHandler(IQueryExecutor queryExecutor, ICommandExecutor commandExecutor, TimeProvider timeProvider) : ICommandHandler<CreateMissingSensorsCommand>
    {
        public async Task HandleAsync(CreateMissingSensorsCommand command, CancellationToken cancellationToken = default)
        {
            var allSensors = await queryExecutor.ExecuteAsync(new GetAllExternalSensorsQuery(), cancellationToken);
            var unknownExternalSensorIds = command.ExternalSensorIds.Distinct().Except(allSensors.Select(s => s.ExternalId)).ToArray();
            foreach (var unknownExternalSensorId in unknownExternalSensorIds)
            {
                await commandExecutor.ExecuteAsync(new CreateSensorCommand(unknownExternalSensorId, unknownExternalSensorId, "This sensor need to be assigned to a zone", timeProvider.GetUtcNow().UtcDateTime), cancellationToken);
            }
        }
    }

    [RequireReporterRole]
    public record CreateMissingSensorsCommand(IEnumerable<string> ExternalSensorIds);
}
