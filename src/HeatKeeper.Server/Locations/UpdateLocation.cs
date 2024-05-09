namespace HeatKeeper.Server.Locations
{
    public class UpdateLocationCommandHandler : ICommandHandler<UpdateLocationCommand>
    {
        private readonly IDbConnection dbConnection;
        private readonly ISqlProvider sqlProvider;

        public UpdateLocationCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            this.dbConnection = dbConnection;
            this.sqlProvider = sqlProvider;
        }

        public async Task HandleAsync(UpdateLocationCommand command, CancellationToken cancellationToken = default)
        {
            await dbConnection.ExecuteAsync(sqlProvider.UpdateLocation, command).ConfigureAwait(false);
        }
    }

    [RequireAdminRole]
    public record UpdateLocationCommand(long Id, string Name, string Description, long? DefaultOutsideZoneId, long? DefaultInsideZoneId);

}
