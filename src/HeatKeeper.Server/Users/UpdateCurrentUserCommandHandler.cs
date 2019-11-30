using CQRS.Command.Abstractions;
using DbReader;
using HeatKeeper.Server.Database;
using HeatKeeper.Server.Security;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace HeatKeeper.Server.Users
{
    public class UpdateCurrentUserCommandHandler : ICommandHandler<UpdateCurrentUserCommand>
    {
        private readonly IDbConnection dbConnection;
        private readonly ISqlProvider sqlProvider;
        private readonly IUserContext userContext;

        public UpdateCurrentUserCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider, IUserContext userContext)
        {
            this.dbConnection = dbConnection;
            this.sqlProvider = sqlProvider;
            this.userContext = userContext;
        }

        public async Task HandleAsync(UpdateCurrentUserCommand command, CancellationToken cancellationToken = default)
        {
            await dbConnection.ExecuteAsync(sqlProvider.UpdateCurrentUser, new { userContext.Id, command.Email, command.FirstName, command.LastName });
        }
    }


    [RequireUserRole]
    public class UpdateCurrentUserCommand : UserCommand
    {
    }
}
