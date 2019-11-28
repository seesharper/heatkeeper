using HeatKeeper.Server.Host;
using HeatKeeper.Server.WebApi.Tests.Transactions;
using LightInject;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using System;

namespace HeatKeeper.Server.WebApi.Tests
{
    public class TestBase : IDisposable
    {
        public TestBase()
        {
            Factory = new WebApplicationFactory<Startup>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestContainer<IServiceContainer>(c => c.RegisterRollbackBehavior());
            });
        }

        public WebApplicationFactory<Startup> Factory { get; }

        public void Dispose()
        {
            Factory.Dispose();
        }
    }
}
