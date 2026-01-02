namespace HeatKeeper.Server.Caching;

using Microsoft.Extensions.Primitives;

public interface ICacheInvalidator<TKey>
{
    IChangeToken GetTokenForAll();
    IChangeToken GetToken(TKey key);

    void InvalidateAll();
    void Invalidate(TKey key);

    /// Cleanup only: forget bookkeeping for a key when its cache entry is gone.
    void Forget(TKey key);
}
