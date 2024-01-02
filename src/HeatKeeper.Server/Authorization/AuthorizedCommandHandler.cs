using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using Microsoft.Extensions.Logging;

namespace HeatKeeper.Server.Authorization;

public class AuthorizedCommandHandler<TCommand> : ICommandHandler<TCommand>
{
    private readonly ICommandHandler<TCommand> _commandHandler;
    private readonly IUserContext _userContext;
    private readonly ILogger<AuthorizedCommandHandler<TCommand>> _logger;
    private static readonly RequireRoleAttribute RoleAttribute = typeof(TCommand).GetRoleAttribute();

    public AuthorizedCommandHandler(ICommandHandler<TCommand> commandHandler, IUserContext userContext, ILogger<AuthorizedCommandHandler<TCommand>> logger)
    {
        _commandHandler = commandHandler;
        _userContext = userContext;
        _logger = logger;
    }

    public async Task HandleAsync(TCommand command, CancellationToken cancellationToken = default)
    {
        if (RoleAttribute.IsSatisfiedBy(_userContext.Role))
        {
            _logger.LogDebug("Successfully authorized access to '{typeof(TCommand)}' for user '{_userContext.Email}({_userContext.Role})'", typeof(TCommand), _userContext.Email, _userContext.Role);
            await _commandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            throw new AuthorizationFailedException($"Failed to authorize access to '{command.GetType()}' for user '{_userContext.Email}({_userContext.Role})'");
        }
    }
}
