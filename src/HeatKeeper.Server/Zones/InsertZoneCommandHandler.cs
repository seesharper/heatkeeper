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
    public class CreateZoneCommandHandler : ICommandHandler<CreateZoneCommand>
    {
        private readonly IDbConnection dbConnection;
        private readonly ISqlProvider sqlProvider;

        public CreateZoneCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            this.dbConnection = dbConnection;
            this.sqlProvider = sqlProvider;
        }

        public async Task HandleAsync(CreateZoneCommand command, CancellationToken cancellationToken = default)
        {
            await ((DbCommand)dbConnection.CreateCommand(sqlProvider.InsertZone, command)).ExecuteNonQueryAsync();
        }
    }

    [RequireUserRole]
    public class CreateZoneCommand : ZoneCommand
    {
        public CreateZoneCommand(string name, string description) : base(name, description)
        {

        }
    }
}
