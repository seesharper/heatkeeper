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
using InfluxDB.Client.Core.Exceptions;
using InfluxDB.Client.Writes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
namespace HeatKeeper.Server.Export;

public class ExportMeasurementsToInfluxDbCommandHandler : ICommandHandler<ExportMeasurementsToInfluxDbCommand>
{
    private readonly IInfluxDBClient _influxDBClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ExportMeasurementsToInfluxDbCommandHandler> _logger;

    public ExportMeasurementsToInfluxDbCommandHandler(IInfluxDBClient influxDBClient, IConfiguration configuration, ILogger<ExportMeasurementsToInfluxDbCommandHandler> logger)
    {
        _influxDBClient = influxDBClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task HandleAsync(ExportMeasurementsToInfluxDbCommand command, CancellationToken cancellationToken = default)
    {
        var measurementsGroupedByRetentionPolicy = command.MeasurementsToExport.GroupBy(m => m.RetentionPolicy);

        var writeApi = _influxDBClient.GetWriteApiAsync();

        foreach (var group in measurementsGroupedByRetentionPolicy)
        {
            foreach (var measurement in group)
            {
                PointData point = PointData.Measurement(measurement.MeasurementTypeName)
                .Field("value", measurement.Value)
                .Tag("location", measurement.Location)
                .Tag("zone", measurement.Zone)
                .Timestamp(measurement.Created, WritePrecision.Ms);
                try
                {
                    await writeApi.WritePointAsync(point, Enum.GetName(typeof(RetentionPolicy), group.Key), _configuration.GetInfluxDbOrganization(), cancellationToken);
                }
                catch (UnprocessableEntityException ex)
                {
                    _logger.LogWarning(ex, "Failed to export measurement");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to export measurement");
                }

            }
        }
    }
}

[RequireBackgroundRole]
public record ExportMeasurementsToInfluxDbCommand(MeasurementToExport[] MeasurementsToExport);
