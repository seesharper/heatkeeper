namespace HeatKeeper.Server.Users;

[RequireAdminRole]
public record AssignLocationToUserCommand([property: JsonIgnore] long UserId, long LocationId);

public class AssignLocationToUserCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<AssignLocationToUserCommand>
{
    public async Task HandleAsync(AssignLocationToUserCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.InsertUserLocation, command);
}