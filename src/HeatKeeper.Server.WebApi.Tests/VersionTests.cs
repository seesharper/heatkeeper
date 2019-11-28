using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace HeatKeeper.Server.WebApi.Tests
{
    public class VersionTests : TestBase
    {
        [Fact]
        public async Task ShouldGetVersion()
        {
            var request = new HttpRequestBuilder()
            .WithMethod(HttpMethod.Get)
            .AddRequestUri("api/version")
            .Build();

            var client = Factory.CreateClient();
            var response = await client.SendAsync(request);
            var version = await response.ContentAs<string>();

            version.Should().NotBeEmpty();
        }
    }
}
