using CQRS.Command.Abstractions;
using HeatKeeper.Server.Locations;
using System.Threading;
using System.Threading.Tasks;

namespace HeatKeeper.Server.Zones
{
    public class MaintainDefaultZonesCommandHandler<TCommand> : ICommandHandler<TCommand> where TCommand : ZoneCommand
    {
        private readonly ICommandHandler<TCommand> handler;
        private readonly ICommandExecutor commandExecutor;

        public MaintainDefaultZonesCommandHandler(ICommandHandler<TCommand> handler, ICommandExecutor commandExecutor)
        {
            this.handler = handler;
            this.commandExecutor = commandExecutor;
        }

        public async Task HandleAsync(TCommand command, CancellationToken cancellationToken = default)
        {
            await handler.HandleAsync(command);
            if (command.UseAsDefaultInsideZone)
            {
                await commandExecutor.ExecuteAsync(new UpdateDefaultInsideZoneCommand() { LocationId = command.LocationId, ZoneId = command.ZoneId });
            }
            else
            {
                await commandExecutor.ExecuteAsync(new ClearDefaultInsideZoneCommand() { LocationId = command.LocationId, ZoneId = command.ZoneId });
            }

            if (command.UseAsDefaultOutsideZone)
            {
                await commandExecutor.ExecuteAsync(new UpdateDefaultOutsideZoneCommand() { LocationId = command.LocationId, ZoneId = command.ZoneId });
            }
            else
            {
                await commandExecutor.ExecuteAsync(new ClearDefaultOutsideZoneCommand() { LocationId = command.LocationId, ZoneId = command.ZoneId });
            }
        }
    }
}
