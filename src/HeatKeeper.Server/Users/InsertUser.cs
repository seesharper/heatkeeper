namespace HeatKeeper.Server.Users;

public class InsertUser(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<InsertUserCommand>
{
    public async Task HandleAsync(InsertUserCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.InsertUser, command);
}

public interface IUserCommand : IProblemCommand
{
    string Email { get; }
    string FirstName { get; }
    string LastName { get; }
    bool IsAdmin { get; }
}

[RequireAdminRole]
public record InsertUserCommand(string Email, string FirstName, string LastName, bool IsAdmin, string HashedPassword) : PostCommand;

