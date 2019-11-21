using CQRS.Command.Abstractions;
using CQRS.Query.Abstractions;
using HeatKeeper.Server.Exceptions;
using System.Threading;
using System.Threading.Tasks;

namespace HeatKeeper.Server.Locations
{
    public class ValidatedLocationCommandHandler<TCommand> : ICommandHandler<TCommand> where TCommand : LocationCommand
    {
        private readonly ICommandHandler<TCommand> handler;
        private readonly IQueryExecutor queryExecutor;

        public ValidatedLocationCommandHandler(ICommandHandler<TCommand> handler, IQueryExecutor queryExecutor)
        {
            this.handler = handler;
            this.queryExecutor = queryExecutor;
        }

        public async Task HandleAsync(TCommand command, CancellationToken cancellationToken = default)
        {
            var userExists = await queryExecutor.ExecuteAsync(new LocationExistsQuery(command.Id, command.Name));
            if (userExists)
            {
                throw new HeatKeeperConflictException($"User {command.Name} already exists");
            }
            await handler.HandleAsync(command);
        }
    }

}
