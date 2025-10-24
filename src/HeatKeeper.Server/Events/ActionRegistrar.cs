using System.Reflection;
using LightInject;

namespace HeatKeeper.Server.Events;

/// <summary>
/// Service for registering actions in the DI container.
/// </summary>
public static class ActionRegistrar
{
    /// <summary>
    /// Discovers all types implementing IAction, builds ActionInfo for each using reflection,
    /// registers them in the catalog, and registers them as named services in the DI container.
    /// </summary>
    /// <param name="serviceRegistry">The LightInject service registry</param>
    /// <param name="catalog">The action catalog to populate</param>
    /// <param name="assemblies">Assemblies to search for action types</param>
    public static IServiceRegistry RegisterActions(
        this IServiceRegistry serviceRegistry,
        ActionCatalog catalog,
        params Assembly[] assemblies)
    {
        var allTypes = assemblies.SelectMany(a => a.GetTypes()).ToList();

        // Find all concrete types implementing IAction
        var actionTypes = allTypes.Where(t =>
            t.IsClass &&
            !t.IsAbstract &&
            typeof(IAction).IsAssignableFrom(t)).ToList();

        foreach (var actionType in actionTypes)
        {
            try
            {
                // Build ActionInfo from the type using reflection
                var actionInfo = ActionInfoBuilder.BuildFrom(actionType);

                // Register in catalog
                catalog.Register(actionInfo);

                // Register as named scoped service in LightInject
                serviceRegistry.RegisterScoped(typeof(IAction), actionType, actionInfo.Name);

                Console.WriteLine($"[SETUP] Registered action '{actionInfo.Name}' -> {actionType.Name}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WARN] Could not register action type '{actionType.Name}': {ex.Message}");
            }
        }

        return serviceRegistry;
    }
}