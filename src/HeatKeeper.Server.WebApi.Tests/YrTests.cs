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
        var testLocation = await Factory.CreateTestLocation();
        var queryExecutor = Factory.Services.GetRequiredService<IQueryExecutor>();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var result = await queryExecutor.ExecuteAsync(new GetSunEventsQuery(testLocation.LocationId, today));
        var result2 = await queryExecutor.ExecuteAsync(new GetSunEventsQuery(testLocation.LocationId, today));

        var cacheInvalidator = Factory.Services.GetRequiredService<ICacheInvalidator<GetSunEventsQuery>>();
        cacheInvalidator.Invalidate(new GetSunEventsQuery(testLocation.LocationId, today));
        var result3 = await queryExecutor.ExecuteAsync(new GetSunEventsQuery(testLocation.LocationId, today));
    }


    [Fact]
    public async Task ShouldGetWeatherForecast()
    {
        var queryExecutor = Factory.Services.GetRequiredService<IQueryExecutor>();
        var result = await queryExecutor.ExecuteAsync(new GetLocationForecastQuery(59.9139, 10.7522));
        result.Should().NotBeNull();
        result.Properties.TimeSeries.Should().NotBeNullOrEmpty();
    }
}
