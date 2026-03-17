using System.Reflection;
using CQRS.Command.Abstractions;
using CQRS.Execution;

namespace HeatKeeper.Server.Events;

internal static class CommandExecutorExtensions
{
    private static readonly System.Reflection.MethodInfo _executeScopedMethod =
        typeof(ScopedCommandExecutorExtensions)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .First(m => m.Name == nameof(ScopedCommandExecutorExtensions.ExecuteScopedAsync)
                && m.GetParameters() is { Length: 3 } p
                && p[0].ParameterType == typeof(ICommandExecutor));

    internal static async Task ExecuteDynamicScopedAsync(this ICommandExecutor executor, object command, CancellationToken ct)
    {
        try
        {
            await (Task)_executeScopedMethod.MakeGenericMethod(command.GetType()).Invoke(null, new object[] { executor, command, ct });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Failed to execute command '{command.GetType().Name}': {ex}");
            throw;
        }
    }
}
