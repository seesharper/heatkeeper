using Microsoft.Extensions.Caching.Memory;

namespace HeatKeeper.Server.Caching;

public static class CacheInvalidationExtensions
{
    public static void AddInvalidation<TKey>(
        this ICacheEntry entry,
        TKey key,
        ICacheInvalidator<TKey> invalidator)
        where TKey : notnull
    {
        // Expire entry when either "all" or "this key" is invalidated
        entry.AddExpirationToken(invalidator.GetToken(key));

        // When the cache entry is evicted for any reason, forget the per-key CTS
        entry.RegisterPostEvictionCallback(static (_, _, _, state) =>
        {
            var (inv, k) = ((ICacheInvalidator<TKey> inv, TKey k))state!;
            inv.Forget(k);
        }, (invalidator, key));
    }
}