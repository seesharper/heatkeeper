namespace HeatKeeper.Server.Users;

public class UserValidator(IEmailValidator emailValidator) : IValidator<IUserCommand>
{
    public Task Validate(IUserCommand command)
    {
        if (!emailValidator.Validate(command.Email))
        {
            command.SetProblemResult($"The mail address '{command.Email}' is not correctly formatted.", StatusCodes.Status400BadRequest);
        }

        if (command.FirstName.IsNullOrEmpty())
        {
            command.SetProblemResult("First name is required.", StatusCodes.Status400BadRequest);
        }

        if (command.LastName.IsNullOrEmpty())
        {
            command.SetProblemResult("Last name is required.", StatusCodes.Status400BadRequest);
        }

        return Task.CompletedTask;
    }
}