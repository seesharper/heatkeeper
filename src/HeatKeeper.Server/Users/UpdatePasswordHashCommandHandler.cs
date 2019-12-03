using CQRS.Command.Abstractions;
using DbReader;
using HeatKeeper.Server.Database;
using HeatKeeper.Server.Authorization;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace HeatKeeper.Server.Users
{
    public class UpdatePasswordHashCommandHandler : ICommandHandler<UpdatePasswordHashCommand>
    {
        private readonly IDbConnection dbConnection;
        private readonly ISqlProvider sqlProvider;

        public UpdatePasswordHashCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            this.dbConnection = dbConnection;
            this.sqlProvider = sqlProvider;
        }

        public async Task HandleAsync(UpdatePasswordHashCommand command, CancellationToken cancellationToken = default)
        {
            await dbConnection.ExecuteAsync(sqlProvider.UpdatePasswordHash, command);
        }
    }

    [RequireUserRole]
    public class UpdatePasswordHashCommand
    {
        public UpdatePasswordHashCommand(long userId, string hashedPassword)
        {
            UserId = userId;
            HashedPassword = hashedPassword;
        }

        public long UserId { get; }
        public string HashedPassword { get; }
    }
}
