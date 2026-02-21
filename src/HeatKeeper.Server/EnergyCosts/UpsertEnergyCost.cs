namespace HeatKeeper.Server.EnergyCosts;

[RequireReporterRole]
public record UpsertEnergyCostCommand(
    long SensorId,
    double PowerImport,
    decimal CostInLocalCurrency,
    decimal CostInLocalCurrencyAfterSubsidy,
    decimal CostInLocalCurrencyWithFixedPrice,
    DateTime Hour);

public class UpsertEnergyCostCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<UpsertEnergyCostCommand>
{
    public async Task HandleAsync(UpsertEnergyCostCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.UpsertEnergyCost, command);
}
