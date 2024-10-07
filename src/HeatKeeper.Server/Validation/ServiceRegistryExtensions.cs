using System.Reflection;
using LightInject;

namespace HeatKeeper.Server.Validation;

public static class ServiceRegistryExtensions
{
    public static IServiceRegistry RegisterValidators(this IServiceRegistry registry, Assembly assembly = null)
    {
        var allTypes = assembly?.GetTypes() ?? Assembly.GetCallingAssembly()!.GetTypes();
        var validators = allTypes
            .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidator<>)))
            .ToList();
        foreach (var validator in validators)
        {
            var validatorInterface = validator.GetInterfaces().First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidator<>));
            registry.RegisterSingleton(validatorInterface, validator);
        }
        return registry;
    }
}