using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using CQRS.Query.Abstractions;
using DbReader;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;
using HeatKeeper.Server.Zones;

namespace HeatKeeper.Server.Locations
{
    public class DeleteLocationCommandHandler : ICommandHandler<DeleteLocationCommand>
    {
        private readonly IDbConnection dbConnection;
        private readonly ISqlProvider sqlProvider;
        private readonly ICommandExecutor commandExecutor;
        private readonly IQueryExecutor queryExecutor;

        public DeleteLocationCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider, ICommandExecutor commandExecutor, IQueryExecutor queryExecutor)
        {
            this.dbConnection = dbConnection;
            this.sqlProvider = sqlProvider;
            this.commandExecutor = commandExecutor;
            this.queryExecutor = queryExecutor;
        }

        public async Task HandleAsync(DeleteLocationCommand command, CancellationToken cancellationToken = default)
        {
            // Remove all location users.
            await dbConnection.ExecuteAsync(sqlProvider.DeleteAllUsersFromLocation, command);

            // Remove all zones for this location.
            ZoneInfo[] zones = await queryExecutor.ExecuteAsync(new ZonesByLocationQuery() { LocationId = command.LocationId }, cancellationToken);
            foreach (var zone in zones)
            {
                await commandExecutor.ExecuteAsync(new DeleteZoneCommand() { ZoneId = zone.Id }, cancellationToken);
            }

            //NOTE: We need to delete all programs 

            await dbConnection.ExecuteAsync(sqlProvider.DeleteLocation, command);
        }
    }

    [RequireAdminRole]
    public class DeleteLocationCommand
    {
        public long LocationId { get; set; }
    }
}
