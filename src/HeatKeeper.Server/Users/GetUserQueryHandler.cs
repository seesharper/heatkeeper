using System.Data;
using System.Threading;
using System.Threading.Tasks;
using HeatKeeper.Server.Database;
using DbReader;
using System.Linq;
using CQRS.Query.Abstractions;
using HeatKeeper.Server.Authorization;

namespace HeatKeeper.Server.Users
{
    public class GetUserQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<GetUserQuery, GetUserQueryResult>
    {
        public async Task<GetUserQueryResult> HandleAsync(GetUserQuery query, CancellationToken cancellationToken = default) 
            => (await dbConnection.ReadAsync<GetUserQueryResult>(sqlProvider.GetUser, query)).SingleOrDefault();
    }

    [RequireAnonymousRole]
    public record GetUserQuery(string Email) : IQuery<GetUserQueryResult>;

    public record GetUserQueryResult(long Id, string Email, string FirstName, string LastName, bool IsAdmin, string HashedPassword);
}
