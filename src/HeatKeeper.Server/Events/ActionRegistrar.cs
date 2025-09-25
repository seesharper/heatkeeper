using System.Reflection;
using LightInject;

namespace HeatKeeper.Server.Events;

/// <summary>
/// Service for registering actions in the DI container based on ActionCatalog entries.
/// </summary>
public static class ActionRegistrar
{
    /// <summary>
    /// Registers actions from the catalog as named services in the LightInject container.
    /// For each ActionInfo in the catalog, searches for a type with the same name 
    /// and registers it as scoped named service.
    /// </summary>
    /// <param name="serviceRegistry">The LightInject service registry</param>
    /// <param name="catalog">The action catalog containing action metadata</param>
    /// <param name="assemblies">Assemblies to search for action types</param>
    public static IServiceRegistry RegisterActionsFromCatalog(
        this IServiceRegistry serviceRegistry,
        ActionCatalog catalog,
        params Assembly[] assemblies)
    {
        var actionInfos = catalog.List();
        var allTypes = assemblies.SelectMany(a => a.GetTypes()).ToList();

        foreach (var actionInfo in actionInfos)
        {
            // Find type that matches the action name
            var actionType = allTypes.FirstOrDefault(t =>
                t.Name.Equals(actionInfo.Name + "Action", StringComparison.OrdinalIgnoreCase) ||
                t.Name.Equals(actionInfo.Name, StringComparison.OrdinalIgnoreCase));

            if (actionType != null && typeof(IAction).IsAssignableFrom(actionType))
            {
                // Register as named scoped service in LightInject
                serviceRegistry.RegisterScoped(typeof(IAction), actionType, actionInfo.Name);
                Console.WriteLine($"[SETUP] Registered action '{actionInfo.Name}' -> {actionType.Name}");
            }
            else
            {
                Console.WriteLine($"[WARN] Could not find action type for '{actionInfo.Name}'");
            }
        }

        return serviceRegistry;
    }
}