using System;
using HeatKeeper.Abstractions.Configuration;
using Microsoft.Extensions.Configuration;

namespace HeatKeeper.Server.Electricity;


[RequireBackgroundRole]
public record HasElectricalPricesForGivenDateQuery(DateTime UtcDateTime, string Area) : IQuery<bool>;

public class HasElectricalPricesForGivenDate : IQueryHandler<HasElectricalPricesForGivenDateQuery, bool>
{

    private readonly IConfiguration _configuration;

    public HasElectricalPricesForGivenDate(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<bool> HandleAsync(HasElectricalPricesForGivenDateQuery query, CancellationToken cancellationToken = default)
    {
        //TODO Get this from database
        return await Task.FromResult(true);
    }
}
