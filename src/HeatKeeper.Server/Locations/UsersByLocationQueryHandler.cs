using System.Data;
using System.Threading;
using System.Threading.Tasks;
using HeatKeeper.Server.Database;
using DbReader;
using System.Linq;
using CQRS.Query.Abstractions;
using HeatKeeper.Server.Users;
using HeatKeeper.Server.Authorization;

namespace HeatKeeper.Server.Locations
{
    public class UsersByLocationQueryHandler : IQueryHandler<UsersByLocationQuery, User[]>
    {
        private readonly IDbConnection dbConnection;
        private readonly ISqlProvider sqlProvider;

        public UsersByLocationQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            this.dbConnection = dbConnection;
            this.sqlProvider = sqlProvider;
        }

        public async Task<User[]> HandleAsync(UsersByLocationQuery query, CancellationToken cancellationToken = default)
        {
            return (await dbConnection.ReadAsync<User>(sqlProvider.GetUsersByLocation, query)).ToArray();
        }
    }

    [RequireUserRole]
    public class UsersByLocationQuery : IQuery<User[]>
    {
        public long LocationId { get; set; }
    }




}
