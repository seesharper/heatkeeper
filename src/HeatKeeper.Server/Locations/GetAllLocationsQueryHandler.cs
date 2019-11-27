using System.Data;
using System.Threading;
using System.Threading.Tasks;
using HeatKeeper.Server.Database;
using DbReader;
using System.Linq;
using CQRS.Query.Abstractions;
using HeatKeeper.Server.Security;

namespace HeatKeeper.Server.Locations
{
    public class GetAllLocationsQueryHandler : IQueryHandler<GetAllLocationsQuery, Location[]>
    {
        private readonly IDbConnection dbConnection;
        private readonly ISqlProvider sqlProvider;

        public GetAllLocationsQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            this.dbConnection = dbConnection;
            this.sqlProvider = sqlProvider;
        }

        public async Task<Location[]> HandleAsync(GetAllLocationsQuery query, CancellationToken cancellationToken = default)
        {
            return (await dbConnection.ReadAsync<Location>(sqlProvider.GetAllLocations, query)).ToArray();
        }
    }


    [RequireUserRole]
    public class GetAllLocationsQuery : IQuery<Location[]>
    {

    }

    public class Location
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
