using CQRS.Command.Abstractions;
using DbReader;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace HeatKeeper.Server.Sensors
{
    public class RemoveSensorFromZoneCommandHandler : ICommandHandler<RemoveSensorFromZoneCommand>
    {
        private readonly IDbConnection dbConnection;
        private readonly ISqlProvider sqlProvider;

        public RemoveSensorFromZoneCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            this.dbConnection = dbConnection;
            this.sqlProvider = sqlProvider;
        }

        public async Task HandleAsync(RemoveSensorFromZoneCommand command, CancellationToken cancellationToken = default)
        {
            await dbConnection.ExecuteAsync(sqlProvider.RemoveSensorFromZone, command);
        }
    }

    [RequireAdminRole]
    public class RemoveSensorFromZoneCommand
    {
        public long ZoneId { get; set; }

        public long SensorId { get; set; }
    }
}
