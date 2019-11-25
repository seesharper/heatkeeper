using CQRS.Command.Abstractions;
using DbReader;
using HeatKeeper.Server.Database;
using HeatKeeper.Server.Security;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace HeatKeeper.Server.Zones
{
    public class InsertZoneCommandHandler : ICommandHandler<InsertZoneCommand>
    {
        private readonly IDbConnection dbConnection;
        private readonly ISqlProvider sqlProvider;

        public InsertZoneCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            this.dbConnection = dbConnection;
            this.sqlProvider = sqlProvider;
        }

        public async Task HandleAsync(InsertZoneCommand command, CancellationToken cancellationToken = default)
        {
            await ((DbCommand)dbConnection.CreateCommand(sqlProvider.InsertZone, command)).ExecuteNonQueryAsync();
        }
    }

    [RequireUserRole]
    public class InsertZoneCommand : ZoneCommand
    {
        public InsertZoneCommand(string name, string description, long locationId) : base(name, description, locationId)
        {
        }
    }
}
