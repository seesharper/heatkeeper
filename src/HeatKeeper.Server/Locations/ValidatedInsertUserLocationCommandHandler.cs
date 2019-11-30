using CQRS.Command.Abstractions;
using CQRS.Query.Abstractions;
using HeatKeeper.Server.Exceptions;
using System.Threading;
using System.Threading.Tasks;

namespace HeatKeeper.Server.Locations
{
    public class ValidatedInsertUserLocationCommandHandler<TCommand> : ICommandHandler<TCommand> where TCommand : AddUserToLocationCommand
    {
        private readonly ICommandHandler<TCommand> handler;
        private readonly IQueryExecutor queryExecutor;

        public ValidatedInsertUserLocationCommandHandler(ICommandHandler<TCommand> handler, IQueryExecutor queryExecutor)
        {
            this.handler = handler;
            this.queryExecutor = queryExecutor;
        }

        public async Task HandleAsync(TCommand command, CancellationToken cancellationToken = default)
        {
            var locationUserExists = await queryExecutor.ExecuteAsync(new LocationUserExistsQuery(command.LocationId, command.UserId));
            if (locationUserExists)
            {
                throw new HeatKeeperConflictException($"An user with user 'userid:{command.UserId}' has already been added to the current location");
            }
            await handler.HandleAsync(command);
        }
    }

    public class ValidatedInsertUserLocationCommand
    {
    }
}
