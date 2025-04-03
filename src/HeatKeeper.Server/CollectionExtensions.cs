
namespace HeatKeeper.Server;

/// <summary>
/// A class which contains useful methods for processing collections.
/// </summary>
public static class CollectionUtilities
{
   
    public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
    {
        if (enumerable != null)
        {
            return !enumerable.Any();
        }
        return true;
    }
}
