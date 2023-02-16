using System.Linq;
using System.Threading.Tasks;
using Janitor;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HeatKeeper.Server.WebApi.Tests;

public class ElectricalPricesTests : TestBase
{
    [Fact]
    public async Task ShouldExportElectricalPrices()
    {
        var janitor = Factory.Services.GetRequiredService<IJanitor>();
        await janitor.Run("ExportElectricalMarketPrices");
    }
}