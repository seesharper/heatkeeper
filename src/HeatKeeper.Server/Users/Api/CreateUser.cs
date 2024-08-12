using HeatKeeper.Server.Authentication;

namespace HeatKeeper.Server.Users.Api;

[RequireAdminRole]
[Post("api/users")]
public record CreateUserCommand(string Email, string FirstName, string LastName, bool IsAdmin, string NewPassword, string ConfirmedPassword) : PostCommand, IUserCommand, IPasswordCommand;

public class CreateUser(ICommandExecutor commandExecutor, IPasswordManager passwordManager) : ICommandHandler<CreateUserCommand>
{
    public async Task HandleAsync(CreateUserCommand command, CancellationToken cancellationToken = default)
    {
        var hashedPassword = passwordManager.GetHashedPassword(command.NewPassword);
        var createUserCommand = new InsertUserCommand(command.Email, command.FirstName, command.LastName, command.IsAdmin, hashedPassword);
        await commandExecutor.ExecuteAsync(createUserCommand, cancellationToken);
    }
}

public class CreateUserValidator(IQueryExecutor queryExecutor) : IValidator<CreateUserCommand>
{
    public async Task Validate(CreateUserCommand command)
    {
        var userExistsQuery = new UserExistsQuery(command.Email);
        if (await queryExecutor.ExecuteAsync(userExistsQuery))
        {
            command.SetProblemResult("User already exists.", StatusCodes.Status409Conflict);
        }
    }
}
