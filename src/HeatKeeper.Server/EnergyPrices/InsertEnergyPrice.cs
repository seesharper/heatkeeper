
namespace HeatKeeper.Server.EnergyPrices;

[RequireBackgroundRole]
public record InsertEnergyPriceCommand(decimal PriceInLocalCurrency, decimal PriceInLocalCurrencyAfterSubsidy, decimal PriceInEuro, DateTime TimeStart, DateTime TimeEnd, string Currency, decimal ExchangeRate, decimal VATRate, long EnergyPriceAreaId);

public class InsertEnergyPriceCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<InsertEnergyPriceCommand>
{
    public async Task HandleAsync(InsertEnergyPriceCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.InsertEnergyPrice, command);
}