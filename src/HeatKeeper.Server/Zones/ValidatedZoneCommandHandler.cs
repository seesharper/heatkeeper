using System.Threading;
using System.Threading.Tasks;
using HeatKeeper.Abstractions.CQRS;
using HeatKeeper.Server.Exceptions;

namespace HeatKeeper.Server.Zones
{
    public class ValidatedZoneCommandHandler<TCommand> : ICommandHandler<TCommand> where TCommand: ZoneCommand
    {
        private readonly ICommandHandler<TCommand> handler;
        private readonly IQueryExecutor queryExecutor;

        public ValidatedZoneCommandHandler(ICommandHandler<TCommand> handler, IQueryExecutor queryExecutor)
        {
            this.handler = handler;
            this.queryExecutor = queryExecutor;
        }

        public async Task HandleAsync(TCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            var zoneExistsQuery = new ZoneExistsQuery(command.Id, command.LocationId, command.Name);
            var zoneExists = await queryExecutor.ExecuteAsync(zoneExistsQuery);
            if (zoneExists)
            {
                throw new HeatKeeperConflictException($"Zone {command.Name} already exists for location {command.Name}");
            }

            await handler.HandleAsync(command, cancellationToken);
        }
    }
}