using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using HeatKeeper.Server.Jobs;
using Xunit;

namespace HeatKeeper.Server.WebApi.Tests;

public class JobsTests : TestBase
{
    [Fact]
    public async Task ShouldGetScheduledJobs()
    {
        var client = Factory.CreateClient();
        var token = await client.AuthenticateAsAdminUser();

        var jobs = await client.Get(new GetScheduledJobsQuery(), token);

        jobs.Should().NotBeEmpty();
    }
}
