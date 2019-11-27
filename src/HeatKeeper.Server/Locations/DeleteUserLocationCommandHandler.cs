using CQRS.Command.Abstractions;
using DbReader;
using HeatKeeper.Server.Database;
using HeatKeeper.Server.Security;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace HeatKeeper.Server.Locations
{
    public class DeleteUserLocationCommandHandler : ICommandHandler<DeleteUserLocationCommand>
    {
        private readonly IDbConnection dbConnection;
        private readonly ISqlProvider sqlProvider;

        public DeleteUserLocationCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            this.dbConnection = dbConnection;
            this.sqlProvider = sqlProvider;
        }

        public async Task HandleAsync(DeleteUserLocationCommand command, CancellationToken cancellationToken = default)
        {
            await dbConnection.ExecuteAsync(sqlProvider.DeleteUserLocation, command);
        }
    }

    [RequireAdminRole]
    public class DeleteUserLocationCommand
    {
        public DeleteUserLocationCommand(long userId, long locationId)
        {
            UserId = userId;
            LocationId = locationId;
        }

        public long UserId { get; }
        public long LocationId { get; }
    }
}
