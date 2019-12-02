using System.Data;
using System.Threading;
using System.Threading.Tasks;
using HeatKeeper.Server.Database;
using DbReader;
using CQRS.Query.Abstractions;
using HeatKeeper.Server.Authorization;

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
        {
            return (await dbConnection.ExecuteScalarAsync<long>(sqlProvider.UserExists, query)) == 1 ? true : false;
        }
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
