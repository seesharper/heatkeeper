using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Query.Abstractions;
using DbReader;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;

public class TemperaturesQueryHandler : IQueryHandler<DashboardTemperaturesQuery, DashboardTemperature[]>
{
    private readonly IDbConnection _dbConnection;
    private readonly ISqlProvider _sqlProvider;
    private readonly IUserContext _userContext;

    public TemperaturesQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider, IUserContext userContext)
    {
        _dbConnection = dbConnection;
        _sqlProvider = sqlProvider;
        _userContext = userContext;
    }


    public async Task<DashboardTemperature[]> HandleAsync(DashboardTemperaturesQuery query, CancellationToken cancellationToken = default)
    {
        var result = (await _dbConnection.ReadAsync<DashboardTemperature>(_sqlProvider.GetDashboardTemperatures, new { UserId = _userContext.Id })).ToArray();
        return result;
    }
}


[RequireUserRole]
public record DashboardTemperaturesQuery(
    long UserId
    ) : IQuery<DashboardTemperature[]>;


public record DashboardTemperature(
    [property: Key] long ZoneId,
    string Name,
    double? Temperature,
    double? Humidity,
    DateTime Updated
    );