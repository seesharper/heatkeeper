namespace HeatKeeper.Server.Events;// This file has been removed - IAction interface is no longer needed.

// Action functionality has been replaced with ActionInfo class for JSON deserialization.
/// <summary>
/// Interface for actions that can be executed by the trigger engine.
/// Actions are retrieved from DI container using keyed services.
/// </summary>
public interface IAction
{
    /// <summary>
    /// Executes the action with the provided parameters.
    /// </summary>
    /// <param name="parameters">The parameters to pass to the action</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>A task representing the async operation</returns>
    Task ExecuteAsync(IReadOnlyDictionary<string, object> parameters, CancellationToken ct);
}