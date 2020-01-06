using CQRS.Command.Abstractions;
using DbReader;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace HeatKeeper.Server.Locations
{
    public class ClearDefaultInsideZoneCommandHandler : ICommandHandler<ClearDefaultInsideZoneCommand>
    {
        private readonly IDbConnection dbConnection;
        private readonly ISqlProvider sqlProvider;

        public ClearDefaultInsideZoneCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            this.dbConnection = dbConnection;
            this.sqlProvider = sqlProvider;
        }

        public async Task HandleAsync(ClearDefaultInsideZoneCommand command, CancellationToken cancellationToken = default)
        {
            await dbConnection.ExecuteAsync(sqlProvider.ClearDefaultInsideZone, command);
        }
    }

    [RequireAdminRole]
    public class ClearDefaultInsideZoneCommand
    {
        public long LocationId { get; set; }

        public long ZoneId { get; set; }
    }
}
