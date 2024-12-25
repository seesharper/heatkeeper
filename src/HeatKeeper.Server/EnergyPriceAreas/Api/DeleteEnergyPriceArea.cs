namespace HeatKeeper.Server.EnergyPriceAreas.Api;

[RequireAdminRole]
[Delete("api/energy-price-areas/{Id}")]
public record DeleteEnergyPriceAreaCommand(long Id) : DeleteCommand;

public class DeleteEnergyPriceAreaCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<DeleteEnergyPriceAreaCommand>
{
    public async Task HandleAsync(DeleteEnergyPriceAreaCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.DeleteEnergyPriceArea, new { command.Id });
}