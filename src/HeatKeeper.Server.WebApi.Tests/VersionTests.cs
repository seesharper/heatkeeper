using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace HeatKeeper.Server.WebApi.Tests
{
    public class VersionTests : TestBase
    {
        [Fact]
        public async Task ShouldGetVersion()
        {
            var client = Factory.CreateClient();

            var version = await client.GetAppVersion();

            version.Value.Should().NotBeEmpty();
        }
    }
}
