namespace HeatKeeper.Server.Locations
{
    public class AddUserToLocationCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<AddUserToLocationCommand>
    {
        public async Task HandleAsync(AddUserToLocationCommand command, CancellationToken cancellationToken = default)
        {
            await dbConnection.ExecuteAsync(sqlProvider.InsertUserLocation, command);
        }
    }

    [RequireAdminRole]
    public record AddUserToLocationCommand(long UserId, long LocationId);

}
