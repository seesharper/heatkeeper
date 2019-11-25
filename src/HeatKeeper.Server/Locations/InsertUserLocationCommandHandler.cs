using CQRS.Command.Abstractions;
using DbReader;
using HeatKeeper.Server.Database;
using HeatKeeper.Server.Security;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace HeatKeeper.Server.Locations
{
    public class InsertUserLocationCommandHandler : ICommandHandler<InsertUserLocationCommand>
    {
        private readonly IDbConnection dbConnection;
        private readonly ISqlProvider sqlProvider;

        public InsertUserLocationCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            this.dbConnection = dbConnection;
            this.sqlProvider = sqlProvider;
        }

        public async Task HandleAsync(InsertUserLocationCommand command, CancellationToken cancellationToken = default)
        {
            await dbConnection.ExecuteAsync(sqlProvider.InsertUserLocation, command);
            command.UserLocationId = await dbConnection.ExecuteScalarAsync<long>(sqlProvider.GetUserLocationId, command);
        }
    }

    [RequireAdminRole]
    public class InsertUserLocationCommand
    {
        public InsertUserLocationCommand(long userId, long locationId)
        {
            UserId = userId;
            LocationId = locationId;
        }

        public long UserId { get; }
        public long LocationId { get; }

        public long UserLocationId { get; set; }
    }
}
