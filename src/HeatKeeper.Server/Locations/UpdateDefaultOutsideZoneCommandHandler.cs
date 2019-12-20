using CQRS.Command.Abstractions;
using DbReader;
using HeatKeeper.Server.Database;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace HeatKeeper.Server.Locations
{
    public class UpdateDefaultOutsideZoneCommandHandler : ICommandHandler<UpdateDefaultOutsideZoneCommand>
    {
        private readonly IDbConnection dbConnection;
        private readonly ISqlProvider sqlProvider;

        public UpdateDefaultOutsideZoneCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            this.dbConnection = dbConnection;
            this.sqlProvider = sqlProvider;
        }

        public async Task HandleAsync(UpdateDefaultOutsideZoneCommand command, CancellationToken cancellationToken = default)
            => await dbConnection.ExecuteAsync(sqlProvider.UpdateDefaultOutsideZone, command);
    }

    public class UpdateDefaultOutsideZoneCommand
    {
        public long LocationId { get; set; }

        public long ZoneId { get; set; }
    }
}
