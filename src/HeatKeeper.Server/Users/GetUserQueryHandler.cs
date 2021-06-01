using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Query.Abstractions;
using DbReader;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;

namespace HeatKeeper.Server.Users
{
    public class GetUserQueryHandler : IQueryHandler<GetUserQuery, GetUserQueryResult>
    {
        private readonly IDbConnection dbConnection;
        private readonly ISqlProvider sqlProvider;

        public GetUserQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            this.dbConnection = dbConnection;
            this.sqlProvider = sqlProvider;
        }

        public async Task<GetUserQueryResult> HandleAsync(GetUserQuery query, CancellationToken cancellationToken = default)
            => (await dbConnection.ReadAsync<GetUserQueryResult>(sqlProvider.GetUser, query)).SingleOrDefault();
    }

    [RequireAnonymousRole]
    public record GetUserQuery(string Email) : IQuery<GetUserQueryResult>;

    public record GetUserQueryResult(long Id, string Email, string FirstName, string LastName, bool IsAdmin, string HashedPassword);
}
