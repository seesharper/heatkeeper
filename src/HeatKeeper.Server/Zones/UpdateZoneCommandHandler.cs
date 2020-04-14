using System.Data;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using DbReader;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;

namespace HeatKeeper.Server.Zones
{
    public class UpdateZoneCommandHandler : ICommandHandler<UpdateZoneCommand>
    {
        private readonly IDbConnection dbConnection;
        private readonly ISqlProvider sqlProvider;

        public UpdateZoneCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            this.dbConnection = dbConnection;
            this.sqlProvider = sqlProvider;
        }

        public async Task HandleAsync(UpdateZoneCommand command, CancellationToken cancellationToken = default(CancellationToken))
            => await dbConnection.ExecuteAsync(sqlProvider.UpdateZone, command).ConfigureAwait(false);
    }

    /// <summary>
    /// Updates the name and the description of the given zone.
    /// </summary>
    [RequireAdminRole]
    public class UpdateZoneCommand
    {
        public long ZoneId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
