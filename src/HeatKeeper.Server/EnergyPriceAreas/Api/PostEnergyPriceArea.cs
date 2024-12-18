namespace HeatKeeper.Server.EnergyPriceAreas.Api;

[RequireAdminRole]
[Post("api/energy-price-areas")]
public record PostEnergyPriceAreaCommand(string EIC_Code, string Name, string Description, long VATRateId) : PostCommand;

public class PostEnergyPriceAreaCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<PostEnergyPriceAreaCommand>
{
    public async Task HandleAsync(PostEnergyPriceAreaCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.InsertEnergyPriceArea, command);
}