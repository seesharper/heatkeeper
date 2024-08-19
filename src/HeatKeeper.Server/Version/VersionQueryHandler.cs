using System.Reflection;


namespace HeatKeeper.Server.Version;

public class VersionQueryHandler : IQueryHandler<VersionQuery, AppVersion>
{
    public Task<AppVersion> HandleAsync(VersionQuery query, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new AppVersion(Assembly.GetExecutingAssembly().GetCustomAttributes<AssemblyInformationalVersionAttribute>().Single().InformationalVersion));
    }
}

[RequireAnonymousRole]
[Get("api/version")]
public record VersionQuery() : IQuery<AppVersion>;

public record AppVersion(string Value);
