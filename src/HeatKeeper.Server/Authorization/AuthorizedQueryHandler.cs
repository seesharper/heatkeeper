using System.Threading;
using System.Threading.Tasks;
using CQRS.Query.Abstractions;
using Microsoft.Extensions.Logging;

namespace HeatKeeper.Server.Authorization;

public class AuthorizedQueryHandler<TQuery, TResult> : IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
{
    private readonly IQueryHandler<TQuery, TResult> _queryHandler;
    private readonly IUserContext _userContext;
    private readonly ILogger<AuthorizedQueryHandler<TQuery, TResult>> _logger;
    private static readonly RequireRoleAttribute RoleAttribute = typeof(TQuery).GetRoleAttribute();

    public AuthorizedQueryHandler(IQueryHandler<TQuery, TResult> queryHandler, IUserContext userContext, ILogger<AuthorizedQueryHandler<TQuery, TResult>> logger)
    {
        _queryHandler = queryHandler;
        _userContext = userContext;
        _logger = logger;
    }

    public async Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default)
    {
        if (RoleAttribute.IsSatisfiedBy(_userContext.Role))
        {
            _logger.LogDebug("Successfully authorized access to '{typeof(TQuery)}' for user '{_userContext.Email}({_userContext.Role})'", typeof(TQuery), _userContext.Email, _userContext.Role);
            return await _queryHandler.HandleAsync(query, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            throw new AuthorizationFailedException($"Failed to authorize access to '{query.GetType()}' for user '{_userContext.Email}({_userContext.Role})'");
        }
    }
}
