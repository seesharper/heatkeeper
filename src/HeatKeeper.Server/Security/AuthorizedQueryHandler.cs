using System.Threading;
using System.Threading.Tasks;
using CQRS.Query.Abstractions;
using HeatKeeper.Server.Users;
using HeatKeeper.Abstractions.Logging;

namespace HeatKeeper.Server.Security
{
    public class AuthorizedQueryHandler<TQuery, TResult> : IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
    {
        private readonly IQueryHandler<TQuery, TResult> queryHandler;
        private readonly IUserContext userContext;
        private readonly Logger logger;
        private static readonly RequireRoleAttribute RoleAttribute = typeof(TQuery).GetRoleAttribute();

        public AuthorizedQueryHandler(IQueryHandler<TQuery, TResult> queryHandler, IUserContext userContext, Logger logger)
        {
            this.queryHandler = queryHandler;
            this.userContext = userContext;
            this.logger = logger;
        }

        public async Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default)
        {
            if (RoleAttribute.IsSatisfiedBy(userContext.Role))
            {
                logger.Info($"Successfully authorized access to '{query.GetType()}' for user '{userContext.Name}({userContext.Role})'");
                return await queryHandler.HandleAsync(query, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                throw new HeatKeeperSecurityException("");
            }
        }
    }



}
