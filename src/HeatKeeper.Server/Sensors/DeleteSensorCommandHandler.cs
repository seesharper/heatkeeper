using System.Data;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using DbReader;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;

namespace HeatKeeper.Server.Sensors
{
    public class DeleteSensorCommandHandler : ICommandHandler<DeleteSensorCommand>
    {
        private readonly IDbConnection dbConnection;
        private readonly ISqlProvider sqlProvider;

        public DeleteSensorCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            this.dbConnection = dbConnection;
            this.sqlProvider = sqlProvider;
        }

        public async Task HandleAsync(DeleteSensorCommand command, CancellationToken cancellationToken = default)
        {
            await dbConnection.ExecuteAsync(sqlProvider.DeleteSensorMeasurements, command);
            await dbConnection.ExecuteAsync(sqlProvider.DeleteSensor, command);
        }
    }

    [RequireAdminRole]
    public class DeleteSensorCommand
    {
        public long SensorId { get; set; }
    }
}
