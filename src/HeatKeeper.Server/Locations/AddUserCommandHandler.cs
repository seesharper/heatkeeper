using DbReader;
using HeatKeeper.Server.CQRS;
using HeatKeeper.Server.Database;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace HeatKeeper.Server.Locations
{
    public class AddUserCommandHandler : ICommandHandler<AddUserCommand>
    {
        private readonly IDbConnection dbConnection;
        private readonly ISqlProvider sqlProvider;

        public AddUserCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            this.dbConnection = dbConnection;
            this.sqlProvider = sqlProvider;
        }

        public async Task HandleAsync(AddUserCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            await dbConnection.ExecuteAsync(sqlProvider.AddUser, command);
        }
    }

    public class AddUserCommand
    {
        public AddUserCommand(long userId, long locationId)
        {
            UserId = userId;
            LocationId = locationId;
        }

        public long UserId { get; }
        public long LocationId { get; }
    }
}