using HeatKeeper.Server.Host;
using HeatKeeper.Server.WebApi.Tests.Transactions;
using LightInject;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
using System;

namespace HeatKeeper.Server.WebApi.Tests
{
    public class IntegrationTestWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override IHostBuilder CreateHostBuilder()
        {
            var builder = base.CreateHostBuilder();
            builder = builder.ConfigureContainer<IServiceContainer>(c =>
            {
                c.RegisterRollbackBehavior();
            });
            return builder;
        }
    }


    public class TestBase : IDisposable
    {
        public TestBase()
        {
            Factory = new IntegrationTestWebApplicationFactory();
        }

        public IntegrationTestWebApplicationFactory Factory { get; }

        public void Dispose()
        {
            Factory.Dispose();
        }
    }
}
