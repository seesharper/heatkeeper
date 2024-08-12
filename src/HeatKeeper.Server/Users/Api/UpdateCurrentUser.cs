namespace HeatKeeper.Server.Users.Api;

public class UpdateCurrentUser(IDbConnection dbConnection, ISqlProvider sqlProvider, IUserContext userContext) : ICommandHandler<UpdateCurrentUserCommand>
{
    public async Task HandleAsync(UpdateCurrentUserCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.UpdateCurrentUser, new { userContext.Id, command.Email, command.FirstName, command.LastName });
}

[RequireUserRole]
[Patch("api/users")]
public record UpdateCurrentUserCommand(string Email, string FirstName, string LastName) : PatchCommand;
