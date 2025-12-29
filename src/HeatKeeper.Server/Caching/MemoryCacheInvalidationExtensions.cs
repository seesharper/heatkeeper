namespace HeatKeeper.Server.Caching;

using Microsoft.Extensions.Caching.Memory;

public static class MemoryCacheInvalidationExtensions
{
    public static Task<TValue> GetOrCreateInvalidatedAsync<TKey, TValue>(
        this IMemoryCache cache,
        TKey key,
        ICacheInvalidator<TKey> invalidator,
        Func<ICacheEntry, Task<TValue>> factory,
        TimeSpan? absoluteExpirationRelativeToNow = null)
        where TKey : notnull
    {
        return cache.GetOrCreateAsync(key, async entry =>
        {
            // Invalidate: (all keys of this type) OR (this specific key)
            entry.AddExpirationToken(invalidator.GetToken(key));

            // Cleanup bookkeeping when entry leaves the cache
            entry.RegisterPostEvictionCallback(static (_, _, _, state) =>
            {
                var (inv, k) = ((ICacheInvalidator<TKey> inv, TKey k))state!;
                inv.Forget(k);
            }, (invalidator, key));

            // Optional TTL safety net
            if (absoluteExpirationRelativeToNow is { } ttl)
                entry.AbsoluteExpirationRelativeToNow = ttl;

            return await factory(entry);
        });
    }

    // Convenience overload if you don't care about ICacheEntry
    public static Task<TValue> GetOrCreateInvalidatedAsync<TKey, TValue>(
        this IMemoryCache cache,
        TKey key,
        ICacheInvalidator<TKey> invalidator,
        Func<Task<TValue>> factory,
        TimeSpan? absoluteExpirationRelativeToNow = null)
        where TKey : notnull
        => cache.GetOrCreateInvalidatedAsync(
            key,
            invalidator,
            _ => factory(),
            absoluteExpirationRelativeToNow);
}
