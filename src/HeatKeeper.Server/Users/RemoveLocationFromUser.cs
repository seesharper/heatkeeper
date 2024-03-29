using System.Text.Json.Serialization;

namespace HeatKeeper.Server.Users;

[RequireAdminRole]
public record RemoveLocationFromUserCommand([property: JsonIgnore] long UserId, long LocationId);

public class RemoveLocationFromUserCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<RemoveLocationFromUserCommand>
{
    public async Task HandleAsync(RemoveLocationFromUserCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.DeleteUserLocation, command);
}