using System.Data;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using DbReader;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;

namespace HeatKeeper.Server.Sensors
{
    public class RemoveZoneFromSensorCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<RemoveZoneFromSensorCommand>
    {
        public async Task HandleAsync(RemoveZoneFromSensorCommand command, CancellationToken cancellationToken = default)
            => await dbConnection.ExecuteAsync(sqlProvider.RemoveZoneFromSensor, command);
    }

    [RequireAdminRole]
    public record RemoveZoneFromSensorCommand(long SensorId);
}
