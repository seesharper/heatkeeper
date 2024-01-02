using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using HeatKeeper.Abstractions.Configuration;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Measurements;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using Microsoft.Extensions.Configuration;

namespace HeatKeeper.Server.Electricity;

[RequireBackgroundRole]
public record MarketPriceExporterCommand(MarketPrice[] MarketPrices);

public class MarketPriceExporter : ICommandHandler<MarketPriceExporterCommand>
{
    private readonly IInfluxDBClient _influxDBClient;
    private readonly IConfiguration _configuration;

    public MarketPriceExporter(IInfluxDBClient influxDBClient, IConfiguration configuration)
    {
        _influxDBClient = influxDBClient;
        _configuration = configuration;
    }

    public async Task HandleAsync(MarketPriceExporterCommand command, CancellationToken cancellationToken = default)
    {
        var points = command.MarketPrices.Select(mp => CreatePoint(mp)).ToList();
        var writeApi = _influxDBClient.GetWriteApiAsync();

        foreach (var point in points)
        {
            await writeApi.WritePointAsync(point, nameof(RetentionPolicy.None), _configuration.GetInfluxDbOrganization());
        }

        // await writeApi.WritePointsAsync(points, nameof(RetentionPolicy.None), _configuration.GetInfluxDbOrganization(), cancellationToken);

    }

    private PointData CreatePoint(MarketPrice marketPrice)
    {
        var marketPriceUtc = marketPrice.StartDateTime.ToUniversalTime();


        var pointData = PointData.Measurement(nameof(MeasurementType.ElectricalPricePerkWh))
            .Field("PriceInNOK", marketPrice.PricePerKiloWattHour)
            .Field("PriceInEuro", marketPrice.PricePerKiloWattHour)
            .Field("ExchangeRate", marketPrice.ExchangeRate)
            .Tag("Area", marketPrice.Area)
            .Timestamp(DateTime.SpecifyKind(marketPrice.StartDateTime, DateTimeKind.Utc), WritePrecision.Ms);
        return pointData;
    }
}