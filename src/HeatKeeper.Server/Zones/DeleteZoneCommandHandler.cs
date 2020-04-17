using System.Data;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using DbReader;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;

namespace HeatKeeper.Server.Zones
{
    public class DeleteZoneCommandHandler : ICommandHandler<DeleteZoneCommand>
    {
        private readonly IDbConnection dbConnection;
        private readonly ISqlProvider sqlProvider;

        public DeleteZoneCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            this.dbConnection = dbConnection;
            this.sqlProvider = sqlProvider;
        }

        public async Task HandleAsync(DeleteZoneCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            await dbConnection.ExecuteAsync(sqlProvider.ClearZoneFromAllSensors, command);
            await dbConnection.ExecuteAsync(sqlProvider.ClearZoneFromAllLocations, command);
            await dbConnection.ExecuteAsync(sqlProvider.DeleteZone, command);
        }
    }

    [RequireAdminRole]
    public class DeleteZoneCommand
    {
        public long ZoneId { get; set; }
    }
}
