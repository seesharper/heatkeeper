using System;
using CQRS.AspNet.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;
using Org.BouncyCastle.Crypto.Engines;

namespace HeatKeeper.Server.WebApi.Tests;

public static class HostBuilderConfigurationExtensions
{
    public static FakeTimeProvider GetFakeTimeProvider(this IHostBuilderConfiguration configuration, DateTime now)
    {
        var fakeTimeProvider = new FakeTimeProvider(now);
        configuration.ConfigureServices((services) => services.AddSingleton<TimeProvider>(fakeTimeProvider));
        return fakeTimeProvider;
    }
}