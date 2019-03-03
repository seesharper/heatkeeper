using AutoFixture;
using HeatKeeper.Server.Host;
using HeatKeeper.Server.WebApi.Tests.Transactions;
using LightInject;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Logging;
using System;
using Xunit.Abstractions;

namespace HeatKeeper.Server.WebApi.Tests
{
    public class TestBase : IDisposable
    {
        private IServiceContainer _container;

        public TestBase(ITestOutputHelper testOutputHelper)
        {
            Factory = new WebApplicationFactory<Startup>()
            .WithWebHostBuilder(builder => {
                builder.ConfigureTestContainer<IServiceContainer>(c => {c.EnableRollback(); _container = c;}).ConfigureLogging(loggingBuilder => loggingBuilder.AddProvider(new TestLoggerProvider()));
            });
            testOutputHelper.Capture();
            Fixture = new Fixture();


        }

        public Fixture Fixture {get;}

        public WebApplicationFactory<Startup> Factory { get; }



        public void Dispose()
        {
            _container.Dispose();
        }
    }
}