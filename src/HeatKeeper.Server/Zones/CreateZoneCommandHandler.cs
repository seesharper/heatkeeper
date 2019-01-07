using System.Data;
using System.Threading;
using System.Threading.Tasks;
using HeatKeeper.Server.CQRS;
using DbReader;
using System.Data.Common;
using HeatKeeper.Server.Database;

namespace HeatKeeper.Server.Zones
{
    public class CreateZoneCommandHandler : ICommandHandler<CreateZoneCommand>
    {
        private readonly IDbConnection dbConnection;
        private readonly ISqlProvider sqlProvider;

        public CreateZoneCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            this.dbConnection = dbConnection;
            this.sqlProvider = sqlProvider;
        }

        public async Task HandleAsync(CreateZoneCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            await ((DbCommand)dbConnection.CreateCommand(sqlProvider.InsertZone, command)).ExecuteNonQueryAsync();
        }
    }
}