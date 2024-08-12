using HeatKeeper.Server.Authentication;

namespace HeatKeeper.Server.Users.Api;

[RequireUserRole]
[Patch("api/users/password")]
public record ChangePasswordCommand(string OldPassword, string NewPassword, string ConfirmedPassword) : PatchCommand, IPasswordCommand;

public class ChangePassword(ICommandExecutor commandExecutor, IPasswordManager passwordManager, IUserContext userContext) : ICommandHandler<ChangePasswordCommand>
{
    public async Task HandleAsync(ChangePasswordCommand command, CancellationToken cancellationToken = default)
    {
        var hashedPassword = passwordManager.GetHashedPassword(command.NewPassword);
        await commandExecutor.ExecuteAsync(new UpdatePasswordHashCommand(userContext.Id, hashedPassword), cancellationToken);
    }
}

public class ChangePasswordValidator(IQueryExecutor queryExecutor, IUserContext userContext, IPasswordManager passwordManager) : IValidator<ChangePasswordCommand>
{
    public async Task Validate(ChangePasswordCommand command)
    {
        var user = await queryExecutor.ExecuteAsync(new GetUserQuery(userContext.Email));
        var hashedPassword = user.HashedPassword;
        if (!passwordManager.VerifyPassword(command.OldPassword, hashedPassword))
        {
            command.SetProblemResult("The old password does not match the current password", StatusCodes.Status400BadRequest);
        }

        if (string.Equals(command.NewPassword, command.OldPassword))
        {
            command.SetProblemResult("The new password must be different from the old password.", StatusCodes.Status400BadRequest);
        }
    }
}


