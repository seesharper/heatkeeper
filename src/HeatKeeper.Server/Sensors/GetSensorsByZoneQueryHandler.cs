using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Query.Abstractions;
using DbReader;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;

namespace HeatKeeper.Server.Sensors
{
    public class SensorsByZoneQueryHandler : IQueryHandler<SensorsByZoneQuery, Sensor[]>
    {
        private readonly IDbConnection dbConnection;
        private readonly ISqlProvider sqlProvider;

        public SensorsByZoneQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            this.dbConnection = dbConnection;
            this.sqlProvider = sqlProvider;
        }

        public async Task<Sensor[]> HandleAsync(SensorsByZoneQuery query, CancellationToken cancellationToken = default)
            => (await dbConnection.ReadAsync<Sensor>(sqlProvider.GetSensorsByZone, query)).ToArray();
    }

    /// <summary>
    /// Gets sensors for the given zone in addition to sensors not connected to a zone.
    /// </summary>
    [RequireAdminRole]
    public record SensorsByZoneQuery(long ZoneId) : IQuery<Sensor[]>;

    public record Sensor(long Id, string ExternalId, long? ZoneId, string Name, string Description);
}
