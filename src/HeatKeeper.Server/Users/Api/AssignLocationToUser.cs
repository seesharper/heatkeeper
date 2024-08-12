namespace HeatKeeper.Server.Users.Api;

[RequireAdminRole]
[Patch("/api/users/{UserId}/assignLocation")]
public record AssignLocationToUserCommand([property: JsonIgnore] long UserId, long LocationId) : PatchCommand;  

public class AssignLocationToUser(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<AssignLocationToUserCommand>
{
    public async Task HandleAsync(AssignLocationToUserCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.InsertUserLocation, command);
}