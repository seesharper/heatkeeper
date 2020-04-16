using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using DbReader;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;
using HeatKeeper.Server.Validation;

namespace HeatKeeper.Server.Users
{
    public class ValidatedDeleteUserCommandHandler : ICommandHandler<DeleteUserCommand>
    {
        private readonly ICommandHandler<DeleteUserCommand> handler;
        private readonly IUserContext userContext;

        public ValidatedDeleteUserCommandHandler(ICommandHandler<DeleteUserCommand> handler, IUserContext userContext)
        {
            this.handler = handler;
            this.userContext = userContext;
        }

        public async Task HandleAsync(DeleteUserCommand command, CancellationToken cancellationToken = default)
        {
            if (command.UserId == userContext.Id)
            {
                throw new ValidationFailedException("The current user cannot be deleted");
            }
            await handler.HandleAsync(command).ConfigureAwait(false);
        }
    }
}
