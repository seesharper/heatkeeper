using System;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using CQRS.Query.Abstractions;
using HeatKeeper.Server.Authorization;

namespace HeatKeeper.Server.Electricity;

[RequireBackgroundRole]
public record ExportAllMarketPricesCommand();

public class ExportAllMarketPrices : ICommandHandler<ExportAllMarketPricesCommand>
{
    private string[] _areas = new string[] { "NO1", "NO2", "NO3", "NO4", "NO5" };

    private readonly ICommandExecutor _commandExecutor;
    private readonly IQueryExecutor _queryExecutor;

    public ExportAllMarketPrices(ICommandExecutor commandExecutor, IQueryExecutor queryExecutor)
    {
        _commandExecutor = commandExecutor;
        _queryExecutor = queryExecutor;
    }

    public async Task HandleAsync(ExportAllMarketPricesCommand command, CancellationToken cancellationToken = default)
    {
        var tomorrow = DateTime.UtcNow.AddDays(1);

        foreach (var area in _areas)
        {
            await ExportMarketPrices(DateTime.UtcNow, area, cancellationToken);
        }

        foreach (var area in _areas)
        {
            await ExportMarketPrices(tomorrow, area, cancellationToken);
        }
    }

    private async Task ExportMarketPrices(DateTime tomorrow, string area, CancellationToken cancellationToken)
    {
        var hasMarketPrices = await _queryExecutor.ExecuteAsync(new HasElectricalPricesForGivenDateQuery(tomorrow, area));
        if (hasMarketPrices)
        {
            return;
        }
        var marketPrices = await _queryExecutor.ExecuteAsync(new GetMarketPricesQuery(tomorrow, area), cancellationToken);
        await _commandExecutor.ExecuteAsync(new MarketPriceExporterCommand(marketPrices), cancellationToken);
    }
}