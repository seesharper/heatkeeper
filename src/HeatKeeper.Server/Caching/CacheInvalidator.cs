namespace HeatKeeper.Server.Caching;

using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Extensions.Primitives;

public sealed class CacheInvalidator<TKey> : ICacheInvalidator<TKey>
    where TKey : notnull
{
    private CancellationTokenSource _allCts = new();
    private readonly ConcurrentDictionary<TKey, CancellationTokenSource> _perKey = new();

    public IChangeToken GetTokenForAll() =>
        new CancellationChangeToken(_allCts.Token);

    public IChangeToken GetToken(TKey key)
    {
        var cts = _perKey.GetOrAdd(key, _ => new CancellationTokenSource());

        return new CompositeChangeToken(new IChangeToken[]
        {
            new CancellationChangeToken(_allCts.Token),
            new CancellationChangeToken(cts.Token)
        });
    }

    public void InvalidateAll()
    {
        var prev = Interlocked.Exchange(ref _allCts, new CancellationTokenSource());
        prev.Cancel();
        prev.Dispose();
    }

    public void Invalidate(TKey key)
    {
        if (_perKey.TryRemove(key, out var cts))
        {
            cts.Cancel();
            cts.Dispose();
        }
    }

    public void Forget(TKey key)
    {
        // Cleanup only: do NOT cancel here (the entry is already leaving the cache).
        if (_perKey.TryRemove(key, out var cts))
        {
            cts.Dispose();
        }
    }
}
