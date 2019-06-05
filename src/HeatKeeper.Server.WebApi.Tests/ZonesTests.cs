using System;
using System.Threading.Tasks;
using AutoFixture;
using HeatKeeper.Abstractions.CQRS;
using HeatKeeper.Server.Host;
using HeatKeeper.Server.Host.Locations;
using HeatKeeper.Server.Host.Zones;
using HeatKeeper.Server.WebApi.Tests.Transactions;
using LightInject;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Xunit;
using Xunit.Abstractions;

namespace HeatKeeper.Server.WebApi.Tests
{
    public class ZonesTests : TestBase
    {
        public ZonesTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }




        [Fact]
        public async Task ShouldGetZonesForLocation()
        {
            var client = Factory.CreateClient();
            await client.GetZones(12);

        }


    }
}
