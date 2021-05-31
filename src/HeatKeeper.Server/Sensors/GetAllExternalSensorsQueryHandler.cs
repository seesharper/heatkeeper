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
    public class GetAllExternalSensorsQueryHandler : IQueryHandler<GetAllExternalSensorsQuery, ExternalSensorQueryResult[]>
    {
        private readonly IDbConnection dbConnection;
        private readonly ISqlProvider sqlProvider;

        public GetAllExternalSensorsQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            this.dbConnection = dbConnection;
            this.sqlProvider = sqlProvider;
        }

        public async Task<ExternalSensorQueryResult[]> HandleAsync(GetAllExternalSensorsQuery query, CancellationToken cancellationToken = default)
        {
            var result = await dbConnection.ReadAsync<ExternalSensorQueryResult>(sqlProvider.GetAllExternalSensors, query);
            return result.ToArray();
        }
    }

    [RequireReporterRole]
    public record GetAllExternalSensorsQuery() : IQuery<ExternalSensorQueryResult[]>;
    
    public record ExternalSensorQueryResult(string ExternalId);
}
