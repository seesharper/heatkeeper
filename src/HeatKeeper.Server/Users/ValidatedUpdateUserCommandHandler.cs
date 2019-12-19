using CQRS.Command.Abstractions;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;
using HeatKeeper.Server.Validation;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace HeatKeeper.Server.Users
{
    public class ValidatedUpdateUserCommandHandler : ICommandHandler<UpdateUserCommand>
    {
        private readonly ICommandHandler<UpdateUserCommand> handler;
        private readonly IUserContext userContext;

        public ValidatedUpdateUserCommandHandler(ICommandHandler<UpdateUserCommand> handler, IUserContext userContext)
        {
            this.handler = handler;
            this.userContext = userContext;
        }

        public async Task HandleAsync(UpdateUserCommand command, CancellationToken cancellationToken = default)
        {
            if (command.UserId == userContext.Id && command.IsAdmin != userContext.IsAdmin)
            {
                throw new ValidationFailedException("The current user cannot change its own admin status");
            }
            await handler.HandleAsync(command).ConfigureAwait(false);
        }
    }


}
