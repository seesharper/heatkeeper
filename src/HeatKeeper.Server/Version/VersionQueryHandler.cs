using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Query.Abstractions;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;


namespace HeatKeeper.Server.Version
{
    public class VersionQueryHandler : IQueryHandler<VersionQuery, AppVersion>
    {
        public Task<AppVersion> HandleAsync(VersionQuery query, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new AppVersion(Assembly.GetExecutingAssembly().GetCustomAttributes<AssemblyInformationalVersionAttribute>().Single().InformationalVersion));
        }
    }

    [RequireAnonymousRole]
    public record VersionQuery() : IQuery<AppVersion>;

    public record AppVersion(string Value);
}
