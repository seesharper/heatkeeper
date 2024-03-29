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
    public class UsersByLocationQueryHandler : IQueryHandler<UsersByLocationQuery, UserInfo[]>
    {
        private readonly IDbConnection dbConnection;
        private readonly ISqlProvider sqlProvider;

        public UsersByLocationQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            this.dbConnection = dbConnection;
            this.sqlProvider = sqlProvider;
        }

        public async Task<UserInfo[]> HandleAsync(UsersByLocationQuery query, CancellationToken cancellationToken = default)
        {
            return (await dbConnection.ReadAsync<UserInfo>(sqlProvider.GetUsersByLocation, query)).ToArray();
        }
    }

    [RequireUserRole]
    public class UsersByLocationQuery : IQuery<UserInfo[]>
    {
        public long LocationId { get; set; }
    }




}
