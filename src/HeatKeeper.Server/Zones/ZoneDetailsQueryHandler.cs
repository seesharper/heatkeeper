using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Query.Abstractions;
using DbReader;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;

namespace HeatKeeper.Server.Zones
{
    public class ZoneDetailsQueryHandler : IQueryHandler<ZoneDetailsQuery, ZoneDetails>
    {
        private readonly IDbConnection _dbConnection;
        private readonly ISqlProvider _sqlProvider;

        public ZoneDetailsQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            _dbConnection = dbConnection;
            _sqlProvider = sqlProvider;
        }

        public async Task<ZoneDetails> HandleAsync(ZoneDetailsQuery query, CancellationToken cancellationToken = default)
            => (await _dbConnection.ReadAsync<ZoneDetails>(_sqlProvider.GetZoneDetails, query)).Single();
    }

    [RequireAdminRole]
    public class ZoneDetailsQuery : IQuery<ZoneDetails>
    {
        public long ZoneId { get; set; }
    }

    public class ZoneDetails
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string MqttTopic { get; set; }

        public bool IsDefaultOutsideZone { get; set; }

        public bool IsDefaultInsideZone { get; set; }
    }
}
