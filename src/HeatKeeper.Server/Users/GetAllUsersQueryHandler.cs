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
        {
            return (await dbConnection.ReadAsync<User>(sqlProvider.GetAllUsers)).ToArray();
        }
    }

    [RequireUserRole]
    public class AllUsersQuery : IQuery<User[]>
    {
    }

    public class User
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsAdmin { get; set; }
    }
}
