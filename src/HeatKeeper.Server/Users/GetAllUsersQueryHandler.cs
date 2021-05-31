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
    public class AllUsersQueryHandler : IQueryHandler<AllUsersQuery, User[]>
    {
        private readonly IDbConnection dbConnection;
        private readonly ISqlProvider sqlProvider;

        public AllUsersQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            this.dbConnection = dbConnection;
            this.sqlProvider = sqlProvider;
        }

        public async Task<User[]> HandleAsync(AllUsersQuery query, CancellationToken cancellationToken = default)
            => (await dbConnection.ReadAsync<User>(sqlProvider.GetAllUsers)).ToArray();
    }

    [RequireUserRole]
    public record AllUsersQuery() : IQuery<User[]>;

    public record User(long Id, string Email, string FirstName, string LastName, bool IsAdmin);

}
