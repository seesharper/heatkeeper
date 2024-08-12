namespace HeatKeeper.Server.Users;

[RequireUserRole]
public record UpdatePasswordHashCommand(long UserId, string HashedPassword);

public class UpdatePasswordHashCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<UpdatePasswordHashCommand>
{
    public async Task HandleAsync(UpdatePasswordHashCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.UpdatePasswordHash, command);
}
