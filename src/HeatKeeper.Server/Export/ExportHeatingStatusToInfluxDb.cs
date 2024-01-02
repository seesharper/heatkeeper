using System;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using CQRS.Query.Abstractions;
using HeatKeeper.Abstractions.Configuration;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Measurements;
using HeatKeeper.Server.Programs;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Core.Exceptions;
using InfluxDB.Client.Writes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HeatKeeper.Server.Export;


[RequireBackgroundRole]
public record ExportHeatingStatusToInfluxDbCommand(long ZoneId, HeatingStatus HeatingStatus, DateTime Created);

public class ExportHeatingStatusToInfluxDb : ICommandHandler<ExportHeatingStatusToInfluxDbCommand>
{
    private readonly IInfluxDBClient _influxDBClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ExportHeatingStatusToInfluxDb> _logger;
    private readonly IQueryExecutor _queryExecutor;

    public ExportHeatingStatusToInfluxDb(IInfluxDBClient influxDBClient, IConfiguration configuration, ILogger<ExportHeatingStatusToInfluxDb> logger, IQueryExecutor queryExecutor)
    {
        _influxDBClient = influxDBClient;
        _configuration = configuration;
        _logger = logger;
        _queryExecutor = queryExecutor;
    }

    public async Task HandleAsync(ExportHeatingStatusToInfluxDbCommand command, CancellationToken cancellationToken = default)
    {
        var zoneAndLocation = await _queryExecutor.ExecuteAsync(new GetZoneAndLocationByZoneId(command.ZoneId), cancellationToken);

        PointData point = PointData.Measurement(Enum.GetName(typeof(MeasurementType), MeasurementType.ZoneHeatingStatus))
                   .Field("value", command.HeatingStatus == HeatingStatus.On ? 1 : 0)
                   .Tag("location", zoneAndLocation.ZoneName)
                   .Tag("zone", zoneAndLocation.LocationName)
                   .Timestamp(command.Created, WritePrecision.Ms);

        var writeApi = _influxDBClient.GetWriteApiAsync();

        try
        {
            await writeApi.WritePointAsync(point, Enum.GetName(typeof(RetentionPolicy), RetentionPolicy.Day), _configuration.GetInfluxDbOrganization(), cancellationToken);
        }
        catch (UnprocessableEntityException ex)
        {
            _logger.LogWarning(ex, "Failed to export measurement");
        }
    }
}