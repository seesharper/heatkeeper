namespace HeatKeeper.Server;


public static class ReflectionExtensions
{
    public static void SetValue<T>(this T target, string propertyName, object value)
    {
        typeof(T).GetProperty(propertyName).SetValue(target, value);
    }
}