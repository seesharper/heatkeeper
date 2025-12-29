using System;
using CQRS.Query.Abstractions;
using HeatKeeper.Server.Caching;
using HeatKeeper.Server.Yr;
using Microsoft.Extensions.DependencyInjection;

namespace HeatKeeper.Server.WebApi.Tests;


public class YrTests : TestBase
{
    [Fact]
    public async Task ShouldGetSunEvents()
    {
        var queryExecutor = Factory.Services.GetRequiredService<IQueryExecutor>();
        var result = await queryExecutor.ExecuteAsync(new GetSunEventsQuery(59.9139, 10.7522, DateOnly.FromDateTime(DateTime.UtcNow)));
        var result2 = await queryExecutor.ExecuteAsync(new GetSunEventsQuery(59.9139, 10.7522, DateOnly.FromDateTime(DateTime.UtcNow)));

        var cacheInvalidator = Factory.Services.GetRequiredService<ICacheInvalidator<GetSunEventsQuery>>();
        cacheInvalidator.Invalidate(new GetSunEventsQuery(59.9139, 10.7522, DateOnly.FromDateTime(DateTime.UtcNow)));
        var result3 = await queryExecutor.ExecuteAsync(new GetSunEventsQuery(59.9139, 10.7522, DateOnly.FromDateTime(DateTime.UtcNow)));
    }
}