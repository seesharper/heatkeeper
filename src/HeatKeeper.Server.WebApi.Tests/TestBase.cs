using System;
using CQRS.AspNet.Testing;
using HeatKeeper.Server.WebApi.Tests.Transactions;
using LightInject;
using Microsoft.Extensions.Hosting;

namespace HeatKeeper.Server.WebApi.Tests
{
    public class IntegrationTestWebApplicationFactory : TestApplication<Program>
    {
        public IntegrationTestWebApplicationFactory()
        {
        }
    }

    public class TestBase : IDisposable
    {
        public TestBase()
        {
            Factory = new IntegrationTestWebApplicationFactory();
            Factory.ConfigureHostBuilder(hostBuilder => hostBuilder.ConfigureContainer<IServiceContainer>(c =>
            {
                c.RegisterRollbackBehavior();
            }));
        }

        public IntegrationTestWebApplicationFactory Factory { get; }

        public void Dispose() => Factory.Dispose();
    }
}
