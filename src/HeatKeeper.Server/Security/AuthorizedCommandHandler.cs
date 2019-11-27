using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using HeatKeeper.Abstractions.Logging;
using HeatKeeper.Server.Users;

namespace HeatKeeper.Server.Security
{
    public class AuthorizedCommandHandler<TCommand> : ICommandHandler<TCommand>
    {
        private readonly ICommandHandler<TCommand> commandHandler;
        private readonly IUserContext userContext;
        private readonly Logger logger;
        private static readonly RequireRoleAttribute RoleAttribute = typeof(TCommand).GetRoleAttribute();

        public AuthorizedCommandHandler(ICommandHandler<TCommand> commandHandler, IUserContext userContext, Logger logger)
        {
            this.commandHandler = commandHandler;
            this.userContext = userContext;
            this.logger = logger;
        }

        public async Task HandleAsync(TCommand command, CancellationToken cancellationToken = default)
        {
            if (RoleAttribute.IsSatisfiedBy(userContext.Role))
            {
                logger.Info($"Successfully authorized access to '{command.GetType()}' for user '{userContext.Name}({userContext.Role})'");
                await commandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                throw new AuthorizationFailedException($"Failed to authorize access to '{command.GetType()}' for user '{userContext.Name}({userContext.Role})'");
            }

        }
    }
}
