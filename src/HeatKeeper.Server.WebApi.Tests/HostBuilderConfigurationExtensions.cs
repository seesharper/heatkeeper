using System;
using CQRS.AspNet.Testing;
using HeatKeeper.Server.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;

namespace HeatKeeper.Server.WebApi.Tests;

public static class HostBuilderConfigurationExtensions
{
    public static FakeTimeProvider UseFakeTimeProvider(this IHostBuilderConfiguration configuration, DateTime now)
    {
        var fakeTimeProvider = new FakeTimeProvider(now);
        configuration.ConfigureServices((services) => services.AddSingleton<TimeProvider>(fakeTimeProvider));
        return fakeTimeProvider;
    }

    public static void UseTestUserContext(this IHostBuilderConfiguration configuration)
    {
        configuration.ConfigureServices((services) => services.AddSingleton<IUserContext, TestUserContext>());
    }
}