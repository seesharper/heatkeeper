using System.Data;
using System.Threading;
using System.Threading.Tasks;
using HeatKeeper.Abstractions.CQRS;
using HeatKeeper.Server.Database;
using DbReader;
using System.Linq;

namespace HeatKeeper.Server.Users
{
    public class UserExistsQueryHandler : IQueryHandler<UserExistsQuery,bool>
    {
        private readonly IDbConnection dbConnection;
        private readonly ISqlProvider sqlProvider;

        public UserExistsQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            this.dbConnection = dbConnection;
            this.sqlProvider = sqlProvider;
        }

        public async Task<bool> HandleAsync(UserExistsQuery query, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (await dbConnection.ExecuteScalarAsync<long>(sqlProvider.UserExists, query)) == 1 ? true : false;
        }
    }
    public class UserExistsQuery : IQuery<bool>
    {
        public UserExistsQuery(long id, string name)
        {
            Id = id;
            Name = name;
        }

        public long Id { get; }
        public string Name { get; }
    }
}