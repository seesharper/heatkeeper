using System.Data;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Query.Abstractions;
using DbReader;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;

namespace HeatKeeper.Server.Users
{
    public class UserExistsQueryHandler : IQueryHandler<UserExistsQuery, bool>
    {
        private readonly IDbConnection dbConnection;
        private readonly ISqlProvider sqlProvider;

        public UserExistsQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            this.dbConnection = dbConnection;
            this.sqlProvider = sqlProvider;
        }

        public async Task<bool> HandleAsync(UserExistsQuery query, CancellationToken cancellationToken = default)
            => (await dbConnection.ExecuteScalarAsync<long>(sqlProvider.UserExists, query)) == 1;
    }

    [RequireUserRole]
    public class UserExistsQuery : IQuery<bool>
    {
        public UserExistsQuery(long id, string email)
        {
            Id = id;
            Email = email;
        }

        public long Id { get; }
        public string Email { get; }
    }
}
