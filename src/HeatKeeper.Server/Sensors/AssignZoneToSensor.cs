using System.Data;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using DbReader;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;

namespace HeatKeeper.Server.Sensors
{
    public class AssignZoneToSensorCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<AssignZoneToSensorCommand>
    {
        public async Task HandleAsync(AssignZoneToSensorCommand command, CancellationToken cancellationToken = default)
        {
            await dbConnection.ExecuteAsync(sqlProvider.AssignZoneToSensor, command);
        }
    }

    [RequireAdminRole]
    public record AssignZoneToSensorCommand(long SensorId, long ZoneId);
}
