using System.Threading.Tasks;
using Xunit;
namespace HeatKeeper.Server.WebApi.Tests;


public class PushSubscriptionsTests : TestBase
{
    [Fact]
    public async Task ShouldCreatePushSubscription()
    {
        var client = Factory.CreateClient();
        var testLocation = await Factory.CreateTestLocation();

        // client.CreatePushSubscription(testLocation.Id, "https://example.com", "p256dh", "auth")
    }
}