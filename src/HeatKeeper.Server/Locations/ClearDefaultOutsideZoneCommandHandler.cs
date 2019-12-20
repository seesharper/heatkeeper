using CQRS.Command.Abstractions;
using DbReader;
using HeatKeeper.Server.Database;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace HeatKeeper.Server.Locations
{
    public class ClearDefaultOutsideZoneCommandHandler : ICommandHandler<ClearDefaultOutsideZoneCommand>
    {
        private readonly IDbConnection dbConnection;
        private readonly ISqlProvider sqlProvider;

        public ClearDefaultOutsideZoneCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            this.dbConnection = dbConnection;
            this.sqlProvider = sqlProvider;
        }

        public async Task HandleAsync(ClearDefaultOutsideZoneCommand command, CancellationToken cancellationToken = default)
        {
            await dbConnection.ExecuteAsync(sqlProvider.ClearDefaultOutsideZone, command);
        }
    }

    public class ClearDefaultOutsideZoneCommand
    {
        public long LocationId { get; set; }

        public long ZoneId { get; set; }
    }
}
