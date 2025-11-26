using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HeatKeeper.Server.Events;

/// <summary>
/// JSON (de)serialization helpers for triggers, providing save/load functionality.
/// </summary>
public static class TriggerStore
{
    private static JsonSerializerOptions Options => new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    /// <summary>
    /// Saves trigger definitions to a JSON file.
    /// </summary>
    /// <param name="path">The file path to save to</param>
    /// <param name="triggers">The triggers to save</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>A task representing the async operation</returns>
    public static async Task SaveAsync(string path, IEnumerable<TriggerDefinition> triggers, CancellationToken ct)
    {
        await using var fs = File.Create(path);
        await JsonSerializer.SerializeAsync(fs, triggers, Options, ct);
    }

    /// <summary>
    /// Loads trigger definitions from a JSON file.
    /// </summary>
    /// <param name="path">The file path to load from</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>A list of trigger definitions</returns>
    public static async Task<IReadOnlyList<TriggerDefinition>> LoadAsync(string path, CancellationToken ct)
    {
        await using var fs = File.OpenRead(path);
        var list = await JsonSerializer.DeserializeAsync<List<TriggerDefinition>>(fs, Options, ct) ?? new();
        return list;
    }
}