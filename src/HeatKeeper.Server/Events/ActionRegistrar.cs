using System.Reflection;
using LightInject;

namespace HeatKeeper.Server.Events;

/// <summary>
/// Service for registering action commands in the DI container.
/// </summary>
public static class ActionRegistrar
{
    /// <summary>
    /// Discovers all command types with [Action] attribute, builds ActionDetails for each using reflection,
    /// registers them in the catalog, and registers their handlers in the DI container.
    /// </summary>
    /// <param name="serviceRegistry">The LightInject service registry</param>
    /// <param name="catalog">The action catalog to populate</param>
    /// <param name="assemblies">Assemblies to search for action command types</param>
    public static IServiceRegistry RegisterActions(
        this IServiceRegistry serviceRegistry,
        ActionCatalog catalog,
        params Assembly[] assemblies)
    {
        var allTypes = assemblies.SelectMany(a => a.GetTypes()).ToList();

        // Find all types with [Action] attribute (commands)
        var commandTypes = allTypes.Where(t =>
            t.GetCustomAttribute<ActionAttribute>() != null).ToList();

        foreach (var commandType in commandTypes)
        {
            try
            {
                // Build ActionDetails from the command type using reflection
                var actionDetails = ActionDetailsBuilder.BuildFrom(commandType);

                // Register in catalog
                catalog.Register(actionDetails);

                Console.WriteLine($"[SETUP] Registered action '{actionDetails.Name}' (ID: {actionDetails.Id}) -> {commandType.Name}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WARN] Could not register action command type '{commandType.Name}': {ex.Message}");
            }
        }

        return serviceRegistry;
    }
}