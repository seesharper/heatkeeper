using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace HeatKeeper.Server.WebApi.Tests;

public class QueryConsoleTests : TestBase
{
    [Fact]
    public async Task ShouldExecuteDatabaseQuery()
    {

        var client = Factory.CreateClient();
        var testLocation = await Factory.CreateTestLocation();
        var table = await client.ExecuteDatabaseQuery("SELECT * FROM Measurements", testLocation.Token);
        table.Columns.Length.Should().Be(7);
        table.Rows.Length.Should().Be(6);
    }
}