using System.Linq;
using System.Threading.Tasks;
using HeatKeeper.Server.Programs;
using HeatKeeper.Server.Sensors;
using Janitor;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HeatKeeper.Server.WebApi.Tests;


public class ChannelStateTests : TestBase
{
    [Fact]
    public async Task ShouldSetChannelOn()
    {
        await Factory.CreateTestLocation();

        var janitor = Factory.Services.GetService<IJanitor>();

        await janitor.Run("SetChannelStates");
    }
}