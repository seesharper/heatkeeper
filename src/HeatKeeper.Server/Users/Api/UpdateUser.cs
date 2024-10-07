namespace HeatKeeper.Server.Users.Api;

public class UpdateUser(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<UpdateUserCommand>
{
    public async Task HandleAsync(UpdateUserCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.UpdateUser, command);
}

[RequireAdminRole]
[Patch("api/users/{UserId}")]
public record UpdateUserCommand(long UserId, string Email, string FirstName, string LastName, bool IsAdmin) : PatchCommand, IUserCommand;

public class UpdateUserValidator(IQueryExecutor queryExecutor, IUserContext userContext) : IValidator<UpdateUserCommand>
{
    public async Task Validate(UpdateUserCommand command)
    {
        if (command.UserId == userContext.Id && command.IsAdmin != userContext.IsAdmin)
        {
            command.SetProblemResult("The current user cannot change its own admin status", StatusCodes.Status400BadRequest);
        }
    }
}