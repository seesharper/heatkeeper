using CQRS.Command.Abstractions;
using DbReader;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace HeatKeeper.Server.Sensors
{
    public class AddSensorToLocationCommandHandler : ICommandHandler<AddSensorToZoneCommand>
    {
        private readonly IDbConnection dbConnection;
        private readonly ISqlProvider sqlProvider;

        public AddSensorToLocationCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            this.dbConnection = dbConnection;
            this.sqlProvider = sqlProvider;
        }

        public async Task HandleAsync(AddSensorToZoneCommand command, CancellationToken cancellationToken = default)
        {
            await dbConnection.ExecuteAsync(sqlProvider.AddSensorToZone, command);
        }
    }

    [RequireAdminRole]
    public class AddSensorToZoneCommand
    {
        public long SensorId { get; set; }
        public long ZoneId { get; set; }
    }
}
