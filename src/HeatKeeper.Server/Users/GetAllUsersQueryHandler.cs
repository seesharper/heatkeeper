using System.Data;
using System.Threading;
using System.Threading.Tasks;
using HeatKeeper.Server.Database;
using DbReader;
using System.Linq;
using CQRS.Query.Abstractions;
using HeatKeeper.Server.Security;

namespace HeatKeeper.Server.Users
{
    public class AllUsersQueryHandler : IQueryHandler<AllUsersQuery, UserQueryResult[]>
    {
        private readonly IDbConnection dbConnection;
        private readonly ISqlProvider sqlProvider;

        public AllUsersQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            this.dbConnection = dbConnection;
            this.sqlProvider = sqlProvider;
        }

        public async Task<UserQueryResult[]> HandleAsync(AllUsersQuery query, CancellationToken cancellationToken = default)
        {
            return (await dbConnection.ReadAsync<UserQueryResult>(sqlProvider.GetAllUsers)).ToArray();
        }
    }

    [RequireUserRole]
    public class AllUsersQuery : IQuery<UserQueryResult[]>
    {
    }

    public class UserQueryResult
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool IsAdmin { get; set; }
    }
}
