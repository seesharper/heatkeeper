using System.Runtime.CompilerServices;
using HeatKeeper.Server.Host;

namespace HeatKeeper.Server.WebApi.Tests;


public static class TestAppContextInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        AppEnvironment.IsRunningFromTests = true;
    }
}