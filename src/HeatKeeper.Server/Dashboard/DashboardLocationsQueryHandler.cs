using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Query.Abstractions;
using DbReader;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;

namespace HeatKeeper.Server.Dashboard
{
    public class DashboardLocationsQueryHandler : IQueryHandler<DashboardLocationsQuery, DashboardLocation[]>
    {
        private readonly IDbConnection dbConnection;
        private readonly ISqlProvider sqlProvider;

        public DashboardLocationsQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            this.dbConnection = dbConnection;
            this.sqlProvider = sqlProvider;
        }

        public async Task<DashboardLocation[]> HandleAsync(DashboardLocationsQuery query, CancellationToken cancellationToken = default)
        {
            return (await dbConnection.ReadAsync<DashboardLocation>(sqlProvider.GetAllDashboardLocations)).ToArray();
        }
    }

    [RequireUserRole]
    public class DashboardLocationsQuery : IQuery<DashboardLocation[]>
    {
    }

    public class DashboardLocation
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public double? OutsideTemperature { get; set; }

        public double? OutsideHumidity { get; set; }

        public double? InsideTemperature { get; set; }

        public double? InsideHumidity { get; set; }
    }
}
