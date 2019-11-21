using System.Data;
using System.Threading;
using System.Threading.Tasks;
using HeatKeeper.Server.Database;
using DbReader;
using System.Linq;
using CQRS.Query.Abstractions;

namespace HeatKeeper.Server.Locations
{
    public class GetAllLocationsQueryHandler : IQueryHandler<GetAllLocationsQuery, LocationQueryResult[]>
    {
        private readonly IDbConnection dbConnection;
        private readonly ISqlProvider sqlProvider;

        public GetAllLocationsQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            this.dbConnection = dbConnection;
            this.sqlProvider = sqlProvider;
        }

        public async Task<LocationQueryResult[]> HandleAsync(GetAllLocationsQuery query, CancellationToken cancellationToken = default)
        {
            return (await dbConnection.ReadAsync<LocationQueryResult>(sqlProvider.GetAllLocations, query)).ToArray();
        }
    }
}
