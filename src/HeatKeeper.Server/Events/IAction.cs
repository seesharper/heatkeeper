namespace HeatKeeper.Server.Events;

/// <summary>
/// Non-generic marker interface for actions.
/// </summary>
public interface IAction
{
}

/// <summary>
/// Generic interface for actions that can be executed by the trigger engine with strongly-typed parameters.
/// Actions are retrieved from DI container using keyed services.
/// </summary>
/// <typeparam name="TParameters">The strongly-typed parameter object</typeparam>
public interface IAction<in TParameters> : IAction
{
    /// <summary>
    /// Executes the action with the provided strongly-typed parameters.
    /// </summary>
    /// <param name="parameters">The strongly-typed parameters for the action</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>A task representing the async operation</returns>
    Task ExecuteAsync(TParameters parameters, CancellationToken ct);
}