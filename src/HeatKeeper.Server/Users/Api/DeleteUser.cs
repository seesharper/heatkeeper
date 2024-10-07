namespace HeatKeeper.Server.Users.Api;

public class DeleteUser(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<DeleteUserCommand>
{
    public async Task HandleAsync(DeleteUserCommand command, CancellationToken cancellationToken = default)
    {
        await dbConnection.ExecuteAsync(sqlProvider.DeleteUserLocations, command);
        await dbConnection.ExecuteAsync(sqlProvider.DeleteUser, command);
    }
}

[Delete("/api/users/{UserId}")]
[RequireAdminRole]
public record DeleteUserCommand(long UserId) : DeleteCommand;

public class DeleteUserValidator(IUserContext userContext) : IValidator<DeleteUserCommand>
{
    public async Task Validate(DeleteUserCommand command)
    {
        if (command.UserId == userContext.Id)
        {
            command.SetProblemResult("The current user cannot be deleted", StatusCodes.Status400BadRequest);
        }
    }
}