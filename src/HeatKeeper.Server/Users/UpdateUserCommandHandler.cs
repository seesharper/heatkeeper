using DbReader;
using HeatKeeper.Abstractions.CQRS;
using HeatKeeper.Server.Database;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace HeatKeeper.Server.Users
{
    public class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand>
    {
        private readonly IDbConnection dbConnection;
        private readonly ISqlProvider sqlProvider;

        public UpdateUserCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            this.dbConnection = dbConnection;
            this.sqlProvider = sqlProvider;
        }

        public async Task HandleAsync(UpdateUserCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            await dbConnection.ExecuteAsync(sqlProvider.UpdateUser, command);
        }
    }

    public class UpdateUserCommand : UserCommand
    {
        public UpdateUserCommand(long id, string name, string email, bool isAdmin) : base(name, email, isAdmin)
        {
            Id = id;
        }
    }
}