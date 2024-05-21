namespace HeatKeeper.Server.Zones
{
    public class UpdateZone(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<UpdateZoneCommand>
    {
        public async Task HandleAsync(UpdateZoneCommand command, CancellationToken cancellationToken = default)
            => await dbConnection.ExecuteAsync(sqlProvider.UpdateZone, command).ConfigureAwait(false);
    }

    /// <summary>
    /// Updates the name and the description of the given zone.
    /// </summary>
    [RequireAdminRole]
    public record UpdateZoneCommand(long ZoneId, long LocationId, string Name, string Description);
}
