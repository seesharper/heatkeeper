using System.Data;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using DbReader;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;

namespace HeatKeeper.Server.Locations
{
    public class UpdateDefaultInsideZoneCommandHandler : ICommandHandler<UpdateDefaultInsideZoneCommand>
    {
        private readonly IDbConnection dbConnection;
        private readonly ISqlProvider sqlProvider;

        public UpdateDefaultInsideZoneCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            this.dbConnection = dbConnection;
            this.sqlProvider = sqlProvider;
        }

        public async Task HandleAsync(UpdateDefaultInsideZoneCommand command, CancellationToken cancellationToken = default)
            => await dbConnection.ExecuteAsync(sqlProvider.UpdateDefaultInsideZone, command);
    }

    [RequireAdminRole]
    public class UpdateDefaultInsideZoneCommand
    {
        public long LocationId { get; set; }

        public long ZoneId { get; set; }
    }
}
