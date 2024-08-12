using System.Text.RegularExpressions;
using HeatKeeper.Server.Validation;
using Microsoft.AspNetCore.Http;

namespace HeatKeeper.Server.Users;

public class PasswordValidator : IValidator<IPasswordCommand>
{
    public Task Validate(IPasswordCommand command)
    {
        if (!string.Equals(command.NewPassword, command.ConfirmedPassword))
        {
            command.SetProblemResult("The password does not match confirmed password", StatusCodes.Status400BadRequest);
        }

        if (string.IsNullOrWhiteSpace(command.NewPassword))
        {
            command.SetProblemResult("Password should not be empty", StatusCodes.Status400BadRequest);
        }

        var hasNumber = new Regex(@"[0-9]+");
        var hasUpperChar = new Regex(@"[A-Z]+");
        var hasMiniMaxChars = new Regex(@".{8,64}");
        var hasLowerChar = new Regex(@"[a-z]+");
        var hasSymbols = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");

        if (!hasLowerChar.IsMatch(command.NewPassword))
        {
            command.SetProblemResult("Password should contain at least one lower case letter", StatusCodes.Status400BadRequest);
        }
        else if (!hasUpperChar.IsMatch(command.NewPassword))
        {
            command.SetProblemResult("Password should contain at least one upper case letter", StatusCodes.Status400BadRequest);
        }

        else if (!hasMiniMaxChars.IsMatch(command.NewPassword))
        {
            command.SetProblemResult("Password should not be less than 8 or greater than 64 characters", StatusCodes.Status400BadRequest);
        }

        else if (!hasNumber.IsMatch(command.NewPassword))
        {
            command.SetProblemResult("Password should contain at least one numeric value", StatusCodes.Status400BadRequest);
        }

        else if (!hasSymbols.IsMatch(command.NewPassword))
        {
            command.SetProblemResult("Password should contain at least one special case characters", StatusCodes.Status400BadRequest);
        }

        return Task.CompletedTask;
    }
}