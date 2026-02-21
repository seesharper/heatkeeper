using HeatKeeper.Server.EnergyPrices;
using HeatKeeper.Server.EnergyPrices.Api;
using HeatKeeper.Server.Measurements;
using Microsoft.Extensions.Logging;

namespace HeatKeeper.Server.EnergyCosts;

[RequireReporterRole]
public record CalculateEnergyCostsCommand(MeasurementCommand[] Measurements);

public class CalculateEnergyCostsCommandHandler(
    IQueryExecutor queryExecutor,
    ICommandExecutor commandExecutor,
    ILogger<CalculateEnergyCostsCommandHandler> logger) : ICommandHandler<CalculateEnergyCostsCommand>
{
    public async Task HandleAsync(CalculateEnergyCostsCommand command, CancellationToken cancellationToken = default)
    {
        var cumulativeMeasurements = command.Measurements
            .Where(m => m.MeasurementType == MeasurementType.CumulativePowerImport)
            .OrderBy(m => m.Created)
            .ToArray();

        if (cumulativeMeasurements.Length == 0)
            return;

        var groupedBySensor = cumulativeMeasurements.GroupBy(m => m.SensorId);

        foreach (var sensorGroup in groupedBySensor)
        {
            var externalSensorId = sensorGroup.Key;

            var contexts = await queryExecutor.ExecuteAsync(
                new GetSensorEnergyContextQuery(externalSensorId), cancellationToken);

            var context = contexts.FirstOrDefault();
            if (context == null || context.EnergyPriceAreaId == null)
                continue;

            foreach (var measurement in sensorGroup.OrderBy(m => m.Created))
            {
                var previousReadings = await queryExecutor.ExecuteAsync(
                    new GetPreviousCumulativeReadingQuery(externalSensorId, measurement.Created),
                    cancellationToken);

                var previousReading = previousReadings.FirstOrDefault();
                if (previousReading == null)
                    continue;

                var deltaKwh = (measurement.Value - previousReading.Value) / 1000.0;
                if (deltaKwh <= 0)
                    continue;

                var previousHourStart = TruncateToHour(previousReading.Created);
                var currentHourStart = TruncateToHour(measurement.Created);

                if (previousHourStart == currentHourStart)
                {
                    await UpsertCostForHour(context, currentHourStart, deltaKwh, cancellationToken);
                }
                else
                {
                    var hours = new List<DateTime>();
                    var hourCursor = previousHourStart;
                    while (hourCursor < currentHourStart)
                    {
                        hours.Add(hourCursor);
                        hourCursor = hourCursor.AddHours(1);
                    }

                    if (hours.Count == 0)
                        hours.Add(currentHourStart);

                    var kwhPerHour = deltaKwh / hours.Count;
                    foreach (var hour in hours)
                    {
                        await UpsertCostForHour(context, hour, kwhPerHour, cancellationToken);
                    }
                }
            }
        }
    }

    private async Task UpsertCostForHour(
        SensorEnergyContext context,
        DateTime hourStart,
        double kwhForHour,
        CancellationToken cancellationToken)
    {
        var energyPrices = await queryExecutor.ExecuteAsync(
            new GetEnergyPriceForHourQuery(context.EnergyPriceAreaId!.Value, hourStart),
            cancellationToken);

        var energyPrice = energyPrices.FirstOrDefault();

        if (energyPrice == null)
        {
            try
            {
                await commandExecutor.ExecuteAsync(
                    new ImportEnergyPricesCommand(hourStart), cancellationToken);

                energyPrices = await queryExecutor.ExecuteAsync(
                    new GetEnergyPriceForHourQuery(context.EnergyPriceAreaId.Value, hourStart),
                    cancellationToken);
                energyPrice = energyPrices.FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to import energy prices for {Hour}", hourStart);
            }
        }

        var costInLocalCurrency = energyPrice != null
            ? (decimal)kwhForHour * energyPrice.PriceInLocalCurrency
            : 0m;

        var costAfterSubsidy = energyPrice != null
            ? (decimal)kwhForHour * energyPrice.PriceInLocalCurrencyAfterSubsidy
            : 0m;

        var costWithFixedPrice = context.UseFixedEnergyPrice
            ? (decimal)kwhForHour * (decimal)context.FixedEnergyPrice
            : costInLocalCurrency;

        await commandExecutor.ExecuteAsync(new UpsertEnergyCostCommand(
            context.SensorId,
            kwhForHour,
            costInLocalCurrency,
            costAfterSubsidy,
            costWithFixedPrice,
            hourStart), cancellationToken);
    }

    private static DateTime TruncateToHour(DateTime dt)
        => new(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0, dt.Kind);
}
