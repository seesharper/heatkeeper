using System.Data;
using System.Threading;
using System.Threading.Tasks;
using HeatKeeper.Server.Database;
using DbReader;
using System.Linq;
using CQRS.Query.Abstractions;
using HeatKeeper.Server.Security;

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
    public class GetAllExternalSensorsQuery : IQuery<ExternalSensorQueryResult[]>
    {
    }

    public class ExternalSensorQueryResult
    {
        public string ExternalId { get; set; }
    }


}
