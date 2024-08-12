namespace HeatKeeper.Server.Users.Api;

[RequireAdminRole]
[Patch("/api/users/{UserId}/removeLocation")]
public record RemoveLocationFromUserCommand([property: JsonIgnore] long UserId, long LocationId) : PatchCommand;

public class RemoveLocationFromUser(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<RemoveLocationFromUserCommand>
{
    public async Task HandleAsync(RemoveLocationFromUserCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.DeleteUserLocation, command);
}