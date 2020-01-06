using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using CQRS.Query.Abstractions;
using HeatKeeper.Server.Exceptions;
using HeatKeeper.Server.Validation;

namespace HeatKeeper.Server.Zones
{
    public class ValidatedZoneCommandHandler<TCommand> : ICommandHandler<TCommand> where TCommand : ZoneCommand
    {
        private readonly ICommandHandler<TCommand> handler;
        private readonly IQueryExecutor queryExecutor;

        public ValidatedZoneCommandHandler(ICommandHandler<TCommand> handler, IQueryExecutor queryExecutor)
        {
            this.handler = handler;
            this.queryExecutor = queryExecutor;
        }

        public async Task HandleAsync(TCommand command, CancellationToken cancellationToken = default)
        {
            var zoneExistsQuery = new ZoneExistsQuery(command.ZoneId, command.LocationId, command.Name);
            var zoneExists = await queryExecutor.ExecuteAsync(zoneExistsQuery);
            if (zoneExists)
            {
                throw new HeatKeeperConflictException($"Zone {command.Name} already exists for location {command.Name}");
            }

            if (command.UseAsDefaultInsideZone && command.UseAsDefaultOutsideZone)
            {
                throw new ValidationFailedException("A zone cannot be a default outside zone and a default inside zone at the same time.");
            }

            await handler.HandleAsync(command, cancellationToken);
        }
    }
}
