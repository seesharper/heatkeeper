using CQRS.Command.Abstractions;
using DbReader;
using HeatKeeper.Server.Database;
using HeatKeeper.Server.Security;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace HeatKeeper.Server.Locations
{
    public class AddUserToLocationCommandHandler : ICommandHandler<AddUserToLocationCommand>
    {
        private readonly IDbConnection dbConnection;
        private readonly ISqlProvider sqlProvider;

        public AddUserToLocationCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            this.dbConnection = dbConnection;
            this.sqlProvider = sqlProvider;
        }

        public async Task HandleAsync(AddUserToLocationCommand command, CancellationToken cancellationToken = default)
        {
            await dbConnection.ExecuteAsync(sqlProvider.InsertUserLocation, command);
            command.UserLocationId = await dbConnection.ExecuteScalarAsync<long>(sqlProvider.GetUserLocationId, command);
        }
    }

    [RequireAdminRole]
    public class AddUserToLocationCommand
    {
        public AddUserToLocationCommand(long userId)
        {
            UserId = userId;
        }

        public long UserId { get; }
        public long LocationId { get; set; }

        public long UserLocationId { get; set; }
    }
}
