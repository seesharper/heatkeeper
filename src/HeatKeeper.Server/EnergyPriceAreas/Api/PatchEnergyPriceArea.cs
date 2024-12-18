namespace HeatKeeper.Server.EnergyPriceAreas.Api;

[RequireAdminRole]
[Patch("api/energy-price-areas/{Id}")]
public record PatchEnergyPriceAreaCommand(long Id, string EIC_Code, string Name, string Description, long VATRateId) : PatchCommand;

public class PatchEnergyPriceAreaCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<PatchEnergyPriceAreaCommand>
{
    public async Task HandleAsync(PatchEnergyPriceAreaCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.UpdateEnergyPriceArea, command);
}