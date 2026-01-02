
using Microsoft.Extensions.Caching.Memory;
using Org.BouncyCastle.Asn1.Cms;

namespace HeatKeeper.Server.Caching;

public class CachedQueryHandler<TQuery, TResult>(IQueryHandler<TQuery, TResult> handler, IMemoryCache cache, ICacheInvalidator<TQuery> invalidator) : IQueryHandler<TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    public async Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default)
    {
        var cachedAttribute = (MemoryCachedAttribute)typeof(TQuery).GetCustomAttributes(typeof(MemoryCachedAttribute), true).Single();

        return await cache.GetOrCreateInvalidatedAsync(
            query,
            invalidator,
            async () => await handler.HandleAsync(query, cancellationToken),
            absoluteExpirationRelativeToNow: cachedAttribute.GetTimeSpan()
        )!;
    }
}