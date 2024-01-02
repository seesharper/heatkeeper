using HeatKeeper.Abstractions;
using HeatKeeper.Abstractions.Configuration;

namespace HeatKeeper.Server.Host;

public static class WebApplicationExtensions
{
    public static async Task RunBootStrappers(this IHost webApplication)
    {
        var bootStrappers = webApplication.Services.GetServices<IBootStrapper>();
        foreach (var bootStrapper in bootStrappers)
        {
            await bootStrapper.Execute();
        }
    }

    public static void VerifySecret(this IHost webApplication)
    {
        var secret = webApplication.Services.GetService<IConfiguration>().GetSecret();
        if (secret == null)
        {
            throw new InvalidOperationException("No secret found");
        }
    }
}