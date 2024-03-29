using System.Data;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using DbReader;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;

namespace HeatKeeper.Server.Users
{
    public class UpdatePasswordHashCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<UpdatePasswordHashCommand>
    {
        public async Task HandleAsync(UpdatePasswordHashCommand command, CancellationToken cancellationToken = default)
            => await dbConnection.ExecuteAsync(sqlProvider.UpdatePasswordHash, command);
    }

    [RequireUserRole]
    public record UpdatePasswordHashCommand(long UserId, string HashedPassword);
}
