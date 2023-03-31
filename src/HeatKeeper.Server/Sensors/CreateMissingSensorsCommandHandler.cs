using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using CQRS.Query.Abstractions;
using HeatKeeper.Server.Authorization;


namespace HeatKeeper.Server.Sensors
{
    public class CreateMissingSensorsCommandHandler : ICommandHandler<CreateMissingSensorsCommand>
    {
        private readonly IQueryExecutor _queryExecutor;
        private readonly ICommandExecutor _commandExecutor;

        public CreateMissingSensorsCommandHandler(IQueryExecutor queryExecutor, ICommandExecutor commandExecutor)
        {
            _queryExecutor = queryExecutor;
            _commandExecutor = commandExecutor;
        }

        public async Task HandleAsync(CreateMissingSensorsCommand command, CancellationToken cancellationToken = default)
        {
            var allSensors = await _queryExecutor.ExecuteAsync(new GetAllExternalSensorsQuery(), cancellationToken);
            var unknownExternalSensorIds = command.ExternalSensorIds.Distinct().Except(allSensors.Select(s => s.ExternalId)).ToArray();
            foreach (var unknownExternalSensorId in unknownExternalSensorIds)
            {
                await _commandExecutor.ExecuteAsync(new CreateSensorCommand(unknownExternalSensorId, unknownExternalSensorId, "This sensor need to be assigned to a zone"));
            }
        }
    }


    [RequireReporterRole]
    public class CreateMissingSensorsCommand
    {
        public CreateMissingSensorsCommand(IEnumerable<string> externalSensorIds)
        {
            ExternalSensorIds = externalSensorIds;
        }

        public IEnumerable<string> ExternalSensorIds { get; }
    }
}
