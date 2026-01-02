using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HeatKeeper.Server.Caching;


public class MemoryCachedAttribute : Attribute
{
    public virtual TimeSpan? GetTimeSpan() => null;

}


[AttributeUsage(AttributeTargets.Class)]
public class MemoryCached<TExpiration>(int value) : MemoryCachedAttribute where TExpiration : Expiration, new()
{
    public override TimeSpan? GetTimeSpan() => new TExpiration().ToTimeSpan(value);

}


public abstract class Expiration()
{
    public abstract TimeSpan ToTimeSpan(int value);
}

public class HoursBeforeExpiration() : Expiration()
{
    public override TimeSpan ToTimeSpan(int value) => TimeSpan.FromHours(value);
}

public class MinutesBeforeExpiration() : Expiration()
{
    public override TimeSpan ToTimeSpan(int value) => TimeSpan.FromMinutes(value);
}