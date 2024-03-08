using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using DbReader;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;

namespace HeatKeeper.Server.Sensors
{
    public class UpdateSensorCommandHandler : ICommandHandler<UpdateSensorCommand>
    {
        private readonly IDbConnection dbConnection;
        private readonly ISqlProvider sqlProvider;

        public UpdateSensorCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            this.dbConnection = dbConnection;
            this.sqlProvider = sqlProvider;
        }

        public async Task HandleAsync(UpdateSensorCommand command, CancellationToken cancellationToken = default)
            => await dbConnection.ExecuteAsync(sqlProvider.UpdateSensor, command);
    }

    [RequireAdminRole]
    public class UpdateSensorCommand
    {
        public long SensorId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
