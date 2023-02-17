using System;
using System.Net.Http;
using HeatKeeper.Server.Host;
using HeatKeeper.Server.WebApi.Tests.Transactions;
using LightInject;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;

namespace HeatKeeper.Server.WebApi.Tests
{
    public class IntegrationTestWebApplicationFactory : WebApplicationFactory<Program>, IConfigureContainer
    {
        public IntegrationTestWebApplicationFactory(Action<IServiceContainer>? configureContainer = null)
        {
        }

        public Action<IServiceContainer> ConfigureContainer { get; set; }

        protected override IHostBuilder CreateHostBuilder()
        {
            var builder = base.CreateHostBuilder();
            builder = builder.ConfigureContainer<IServiceContainer>(c =>
            {
                c.RegisterRollbackBehavior();
                ConfigureContainer?.Invoke(c);
            });
            return builder;
        }
    }

    public static class WebApplicationFactoryExtensions
    {
        public static HttpClient CreateClient(this IntegrationTestWebApplicationFactory factory, Action<IServiceContainer> configureContainer)
        {
            ((IConfigureContainer)factory).ConfigureContainer = configureContainer;
            return factory.CreateClient();
        }
    }

    public interface IConfigureContainer
    {
        Action<IServiceContainer> ConfigureContainer { get; set; }
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
