using System;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Query.Abstractions;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Configuration;
using InfluxDB.Client;
using Microsoft.Extensions.Configuration;

namespace HeatKeeper.Server.Electricity;


[RequireBackgroundRole]
public record HasElectricalPricesForGivenDateQuery(DateTime UtcDateTime, string Area) : IQuery<bool>;

public class HasElectricalPricesForGivenDate : IQueryHandler<HasElectricalPricesForGivenDateQuery, bool>
{
    private readonly IInfluxDBClient _influxDBClient;

    private readonly IConfiguration _configuration;

    public HasElectricalPricesForGivenDate(IInfluxDBClient influxDBClient, IConfiguration configuration)
    {
        _influxDBClient = influxDBClient;
        _configuration = configuration;
    }

    public async Task<bool> HandleAsync(HasElectricalPricesForGivenDateQuery query, CancellationToken cancellationToken = default)
    {
        var startDateTime = new DateTime(query.UtcDateTime.Year, query.UtcDateTime.Month, query.UtcDateTime.Day, 0, 0, 0, DateTimeKind.Utc).ToString("O");
        var stopDateTime = new DateTime(query.UtcDateTime.Year, query.UtcDateTime.Month, query.UtcDateTime.Day, 23, 59, 0, DateTimeKind.Utc).ToString("O");

        string flux = $"""
        from(bucket: "None")
        |> range(start: {startDateTime}, stop: {stopDateTime})
        |> filter(fn: (r) => r["Area"] == "{query.Area}")
        |> filter(fn: (r) => r["_field"] == "PriceInNOK")
        |> count()
        """;

        var queryApi = _influxDBClient.GetQueryApi();

        var result = await queryApi.QueryAsync(string.Format(flux, startDateTime, stopDateTime), _configuration.GetInfluxDbOrganization());
        return (result.Count > 0);
    }
}
