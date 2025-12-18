using System;
using CQRS.Query.Abstractions;
using HeatKeeper.Server.Yr;
using Microsoft.Extensions.DependencyInjection;

namespace HeatKeeper.Server.WebApi.Tests;


public class YrTests : TestBase
{
    [Fact]
    public async Task ShouldGetSunEvents()
    {
        var queryExecutor = Factory.Services.GetRequiredService<IQueryExecutor>(); 
        var result = await queryExecutor.ExecuteAsync(new GetSunEventsQuery(59.9139, 10.7522, DateTime.UtcNow.Date));
    }
}