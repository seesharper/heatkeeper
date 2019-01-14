using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HeatKeeper.Server.CQRS;

namespace HeatKeeper.Server.Sensors
{
    public class CreateMissingSensorsCommandHandler : ICommandHandler<CreateMissingSensorsCommand>
    {
        private readonly IQueryExecutor queryExecutor;
        private readonly ICommandExecutor commandExecutor;

        public CreateMissingSensorsCommandHandler(IQueryExecutor queryExecutor, ICommandExecutor commandExecutor)
        {
            this.queryExecutor = queryExecutor;
            this.commandExecutor = commandExecutor;
        }

        public async Task HandleAsync(CreateMissingSensorsCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            var allSensors = await queryExecutor.ExecuteAsync(new GetAllExternalSensorsQuery());
            var unknownExternalSensorIds = command.ExternalSensorIds.Distinct().Except(allSensors.Select(s => s.ExternalId)).ToArray();
            foreach (var unknownExternalSensorId in unknownExternalSensorIds)
            {
                await commandExecutor.ExecuteAsync(new CreateSensorCommand(unknownExternalSensorId,"New sensor", "This sensor need to be assigned to a zone"));
            }
        }
    }
}