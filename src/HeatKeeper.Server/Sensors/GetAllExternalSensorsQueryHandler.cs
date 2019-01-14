using System.Data;
using System.Threading;
using System.Threading.Tasks;
using HeatKeeper.Server.CQRS;
using HeatKeeper.Server.Database;
using DbReader;
using System.Linq;

namespace HeatKeeper.Server.Sensors
{
    public class GetAllExternalSensorsQueryHandler : IQueryHandler<GetAllExternalSensorsQuery,ExternalSensorQueryResult[]>
    {
        private readonly IDbConnection dbConnection;
        private readonly ISqlProvider sqlProvider;

        public GetAllExternalSensorsQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            this.dbConnection = dbConnection;
            this.sqlProvider = sqlProvider;
        }

        public async Task<ExternalSensorQueryResult[]> HandleAsync(GetAllExternalSensorsQuery query, CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await dbConnection.ReadAsync<ExternalSensorQueryResult>(sqlProvider.GetAllExternalSensors, query);
            return result.ToArray();
        }
    }
    public class GetAllExternalSensorsQuery : IQuery<ExternalSensorQueryResult[]>
    {
    }

    public class ExternalSensorQueryResult
    {
        public string ExternalId { get; set; }
    }


}