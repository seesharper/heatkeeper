using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;

namespace HeatKeeper.Server.Measurements
{
    public class InsertMeasurementsCommandHandler : ICommandHandler<MeasurementCommand[]>
    {
        private readonly ICommandExecutor commandExecutor;

        public InsertMeasurementsCommandHandler(ICommandExecutor commandExecutor)
        {
            this.commandExecutor = commandExecutor;
        }

        public async Task HandleAsync(MeasurementCommand[] commands, CancellationToken cancellationToken = default)
        {
            foreach (var command in commands)
            {
                await commandExecutor.ExecuteAsync(command);
            }
        }
    }
}
