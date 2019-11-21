using CQRS.Command.Abstractions;
using DbReader;
using HeatKeeper.Server.Database;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace HeatKeeper.Server.Users
{
    public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand>
    {
        private readonly IDbConnection dbConnection;
        private readonly ISqlProvider sqlProvider;

        public CreateUserCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            this.dbConnection = dbConnection;
            this.sqlProvider = sqlProvider;
        }

        public async Task HandleAsync(CreateUserCommand command, CancellationToken cancellationToken = default)
        {
            await dbConnection.ExecuteAsync(sqlProvider.InsertUser, command);
            command.Id = await dbConnection.ExecuteScalarAsync<long>(sqlProvider.GetUserId, new { command.Name });
        }
    }
}
