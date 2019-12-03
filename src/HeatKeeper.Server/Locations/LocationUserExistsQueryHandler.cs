using System.Data;
using System.Threading;
using System.Threading.Tasks;
using HeatKeeper.Server.Database;
using DbReader;
using CQRS.Query.Abstractions;
using HeatKeeper.Server.Authorization;

namespace HeatKeeper.Server.Locations
{
    public class LocationUserExistsQueryHandler : IQueryHandler<LocationUserExistsQuery, bool>
    {
        private readonly IDbConnection dbConnection;
        private readonly ISqlProvider sqlProvider;

        public LocationUserExistsQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            this.dbConnection = dbConnection;
            this.sqlProvider = sqlProvider;
        }

        public async Task<bool> HandleAsync(LocationUserExistsQuery query, CancellationToken cancellationToken = default)
        {
            return (await dbConnection.ExecuteScalarAsync<long>(sqlProvider.LocationUserExists, query)) == 1 ? true : false;
        }
    }

    [RequireAdminRole]
    public class LocationUserExistsQuery : IQuery<bool>
    {
        public LocationUserExistsQuery(long locationId, long userId)
        {
            LocationId = locationId;
            UserId = userId;
        }

        public long LocationId { get; }
        public long UserId { get; }
    }
}
