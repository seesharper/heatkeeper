using Xunit;
using WebPush;
using System;

namespace HeatKeeper.Server.WebApi.Tests;

public class GenerateVAPIDKeys
{
    [Fact]
    public void Generate()
    {
        var keys = VapidHelper.GenerateVapidKeys();
        Console.WriteLine($"Public key: {keys.PublicKey}");
        Console.WriteLine($"Private key: {keys.PrivateKey}");
    }
}
